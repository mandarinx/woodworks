using UnityEngine;
using System.Collections.Generic;
using UnityEditor.Rendering;

public struct TriangleData {
    public int[] indices;
    public float[] x;
    public float[] y;
    public float[] z;

    public TriangleData(int i1, int i2, int i3) {
        indices = new[] {i1, i2, i3};
        x = new float[3];
        y = new float[3];
        z = new float[3];
    }

    public static void SetX(TriangleData td, float x0, float x1, float x2) {
        td.x[0] = x0;
        td.x[1] = x1;
        td.x[2] = x2;
    }

    public static void SetY(TriangleData td, float y0, float y1, float y2) {
        td.y[0] = y0;
        td.y[1] = y1;
        td.y[2] = y2;
    }

    public static void SetZ(TriangleData td, float z0, float z1, float z2) {
        td.z[0] = z0;
        td.z[1] = z1;
        td.z[2] = z2;
    }

    public static Vector3 GetVector(TriangleData td, int i) {
        return new Vector3(td.x[i], td.y[i], td.z[i]);
    }
}

public enum UVMapFnID {
    PROJECTION = 0,
    BARK = 1,
}

public enum AXIS {
    X = 0, 
    Y = 1, 
    Z = 2,
}

public enum SIDE {
    TOP = 0, 
    BOTTOM = 1, 
    RIGHT = 2,
    LEFT = 3,
    FRONT = 4,
    BACK = 5,
}

public delegate void UVMapFn(MaterialData mat, TriangleData td, List<Vector2> uvs);

public class UVMapper : MonoBehaviour {

    // The length of the adjacent side in a right angled triangle
    // with an angle of 45 deg and hypoteneuse of 1.
    // This is the same as if we were to compare the rotation of
    // the vector to some threshold.
    public const float adjacent = 0.70710678f;
    public const float axisAlignThreshold = 2f; // 2 degrees
    public const float sideThreshold = 0.1f;

    private readonly UVMapFn[] mapFunctions = { ProjectionMapping, BarkMapping };

    public static UVMapFn GetMapFunction(UVMapper mapper, UVMapFnID id) {
        return mapper.mapFunctions[(int)id];
    }

    public static void SetUV(MaterialData mat, Mesh mesh, UVMapFn mapFn) {
        List<Vector2> uvs = new List<Vector2>();
        for (int i = 0; i < mesh.vertices.Length; ++i) {
            uvs.Add(Vector2.zero);
        }
        
        for (int i = 0; i < mesh.triangles.Length; i+=3) {
            int t1 = mesh.triangles[i];
            int t2 = mesh.triangles[i + 1];
            int t3 = mesh.triangles[i + 2];
            TriangleData td = new TriangleData(t1, t2, t3);
            Vector3 va = mesh.vertices[t1];
            Vector3 vb = mesh.vertices[t2];
            Vector3 vc = mesh.vertices[t3];
            TriangleData.SetX(td, va.x, vb.x, vc.x);
            TriangleData.SetY(td, va.y, vb.y, vc.y);
            TriangleData.SetZ(td, va.z, vb.z, vc.z);
            mapFn(mat, td, uvs);
        }

        mesh.SetUVs(0, uvs);
    }

    // Projects textures along the cardinal axis.
    // The bark texture is applied to the sides, no
    // matter how far into the mesh you slice.
    public static void ProjectionMapping(MaterialData mat, TriangleData triangle, List<Vector2> uvs) {
        Vector3 a = TriangleData.GetVector(triangle, 0);
        Vector3 b = TriangleData.GetVector(triangle, 1);
        Vector3 c = TriangleData.GetVector(triangle, 2);
        Vector3 normal = Vector3.Cross(a - b, a - c).normalized;
        
        Vector2 uva = Vector2.zero;
        Vector2 uvb = Vector2.zero;
        Vector2 uvc = Vector2.zero;

        // Sides are 0, top/bottom is 1.
        // This info should be provided by the
        // MaterialData object.
        // MaterialIndex.GetTileIndex(mi, materialData, SIDE);
        
        // Tile 0 is the side with bark
        int tile = 0;

        // Check if the normal is within a range of +/- 85 deg
        // of the world Y axis.
        if (IsAlignedY(normal, 85f)) {
            uva = ProjectY(a, mat.halfSize, mat.size);
            uvb = ProjectY(b, mat.halfSize, mat.size);
            uvc = ProjectY(c, mat.halfSize, mat.size);
            // Tile 3 is the edge without bark
            tile = 3;
        }

        // These checks fail when slicing at an angle outside
        // axisAlignThreshold. Could improve the IsAligned functions
        // to accept an axis to project along. That way I could
        // project the texture along the normal of the triangle.
        // how would that translate to UV coords?
        if (IsAlignedX(normal, axisAlignThreshold)) {
            uva = ProjectX(a, mat.halfSize, mat.size);
            uvb = ProjectX(b, mat.halfSize, mat.size);
            uvc = ProjectX(c, mat.halfSize, mat.size);
        }

        if (IsAlignedZ(normal, axisAlignThreshold)) {
            uva = ProjectZ(a, mat.halfSize, mat.size);
            uvb = ProjectZ(b, mat.halfSize, mat.size);
            uvc = ProjectZ(c, mat.halfSize, mat.size);
        }
        
        uvs[triangle.indices[0]] = RemapUVCoordToTile(mat, uva, tile);
        uvs[triangle.indices[1]] = RemapUVCoordToTile(mat, uvb, tile);
        uvs[triangle.indices[2]] = RemapUVCoordToTile(mat, uvc, tile);
    }

    public static void BarkMapping(MaterialData mat, TriangleData triangle, List<Vector2> uvs) {
        Vector3 a = TriangleData.GetVector(triangle, 0);
        Vector3 b = TriangleData.GetVector(triangle, 1);
        Vector3 c = TriangleData.GetVector(triangle, 2);
        Vector3 normal = Vector3.Cross(a - b, a - c).normalized;
        
        Vector2 uva = Vector2.zero;
        Vector2 uvb = Vector2.zero;
        Vector2 uvc = Vector2.zero;

        // Sides are 0, top/bottom is 1. This info should be provided by the
        // MaterialData object. Won't work for bark materials, will it?
        // Don't implement this yet. I need to see how MaterialData matures.
        // MaterialIndex.GetTileIndex(mi, materialData, SIDE);
        int tile = 0;

        if (IsAlignedX(normal, axisAlignThreshold)) {
            uva = ProjectX(a, mat.halfSize, mat.size);
            uvb = ProjectX(b, mat.halfSize, mat.size);
            uvc = ProjectX(c, mat.halfSize, mat.size);
            // Pick a vertex and see if it is a part of the bark
            tile = (a.x < -(mat.halfSize.x - mat.barkThickness.x)) ||
                   (a.x >   mat.halfSize.x - mat.barkThickness.x)
                ? 0
                : 1;
        }

        if (IsAlignedZ(normal, axisAlignThreshold)) {
            uva = ProjectZ(a, mat.halfSize, mat.size);
            uvb = ProjectZ(b, mat.halfSize, mat.size);
            uvc = ProjectZ(c, mat.halfSize, mat.size);
            tile = (a.z < -(mat.halfSize.z - mat.barkThickness.x)) ||
                   (a.z >   mat.halfSize.z - mat.barkThickness.x)
                ? 0
                : 1;
        }

        if (IsAlignedY(normal, axisAlignThreshold)) {
            uva = ProjectY(a, mat.halfSize, mat.size);
            uvb = ProjectY(b, mat.halfSize, mat.size);
            uvc = ProjectY(c, mat.halfSize, mat.size);
            tile = (a.y < -(mat.halfSize.y - mat.barkThickness.y)) ||
                   (a.y >   mat.halfSize.y - mat.barkThickness.y)
                ? 2
                : 3;
        }
        
        uvs[triangle.indices[0]] = RemapUVCoordToTile(mat, uva, tile);
        uvs[triangle.indices[1]] = RemapUVCoordToTile(mat, uvb, tile);
        uvs[triangle.indices[2]] = RemapUVCoordToTile(mat, uvc, tile);
    }

    private static Vector2 ProjectX(Vector3 coord, Vector3 offset, Vector3 size) {
        return new Vector2((coord.z + offset.z) / size.z,
                           (coord.y + offset.y) / size.y);
    }

    private static Vector2 ProjectY(Vector3 coord, Vector3 offset, Vector3 size) {
        return new Vector2((coord.x + offset.x) / size.x,
                           (coord.z + offset.z) / size.z);
    }

    private static Vector2 ProjectZ(Vector3 coord, Vector3 offset, Vector3 size) {
        return new Vector2((coord.x + offset.x) / size.x,
                           (coord.y + offset.y) / size.y);
    }
    
    // Why does this work again?
//    private static SIDE GetSide(Vector3 vector, float threshold) {
//        if (vector.z < threshold && vector.z >= -threshold) {
//            return vector.x > 0 ? SIDE.RIGHT : SIDE.LEFT;
//        }
//        if (vector.x < threshold && vector.x >= -threshold) {
//            return vector.z > 0 ? SIDE.FRONT : SIDE.BACK;
//        }
//        return vector.y > 0 ? SIDE.TOP : SIDE.BOTTOM;
//    }

//    private static SIDE GetSide(Vector3 vector, float threshold) {
//        threshold *= Mathf.Deg2Rad;
//        float rad = Mathf.Acos(Vector3.Dot(vector, Vector3.forward));
//        
//        if (rad > -threshold && rad < threshold) {
//            return SIDE.FRONT;
//        }
//        if (rad > Mathf.PI - threshold) {
//            return SIDE.BACK;
//        }
//        if (rad > Mathf.PI * 0.5f - threshold && 
//            rad < Mathf.PI * 0.5f + threshold) {
//            
//            rad = Mathf.Acos(Vector3.Dot(vector, Vector3.right));
//            
//            if (rad > -threshold && rad < threshold) {
//                return SIDE.RIGHT;
//            }
//            if (rad > Mathf.PI - threshold) {
//                return SIDE.LEFT;
//            }
//
//            if (rad > Mathf.PI * 0.5f - threshold &&
//                rad < Mathf.PI * 0.5f + threshold) {
//
//                rad = Mathf.Acos(Vector3.Dot(vector, Vector3.up));
//
//                if (rad > -threshold && rad < threshold) {
//                    return SIDE.TOP;
//                }
//                if (rad > Mathf.PI - threshold) {
//                    return SIDE.BOTTOM;
//                }
//            }
//        }
//    }

    // Threshold in degrees
    public static bool IsAlignedX(Vector3 dir, float threshold) {
        threshold *= Mathf.Deg2Rad;
        float rad = Mathf.Acos(Vector3.Dot(dir, Vector3.right));
        return (rad > -threshold && rad < threshold) || (rad > Mathf.PI - threshold);
    }

    // Threshold in degrees
    public static bool IsAlignedY(Vector3 dir, float threshold) {
        threshold *= Mathf.Deg2Rad;
        float rad = Mathf.Acos(Vector3.Dot(dir, Vector3.up));
        return (rad > -threshold && rad < threshold) || (rad > Mathf.PI - threshold);
    }

    // Threshold in degrees
    public static bool IsAlignedZ(Vector3 dir, float threshold) {
        threshold *= Mathf.Deg2Rad;
        float rad = Mathf.Acos(Vector3.Dot(dir, Vector3.forward));
        return (rad > -threshold && rad < threshold) || (rad > Mathf.PI - threshold);
    }

    private static Vector2 RemapUVCoordToTile(MaterialData mat, Vector2 uv, int tile) {
        Vector2 offset = mat.tiles[tile * 2];
        Vector2 size = mat.tiles[tile * 2 + 1];
        
        return new Vector2(Map(    uv.x, 0, 1, offset.x, offset.x + size.x),
                           Map(1 - uv.y, 0, 1, offset.y, offset.y + size.y));
    }

    public static bool WithinThreshold(Vector3 v, AXIS axis, float threshold) {
        int a = (int) axis;
        return v[a] < threshold && v[a] > -threshold;
    }
    
    private static float Map(float x, float in_min, float in_max, float out_min, float out_max) {
        return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
    }
}
