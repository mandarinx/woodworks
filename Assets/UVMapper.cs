using UnityEngine;
using System.Collections.Generic;

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
}

public enum UVMapFnID {
    BOX = 0,
    BARK = 1,
}

public enum AXIS { X = 0, Y = 1, Z = 2 }

public delegate void UVMapFn(MaterialData mat, TriangleData td, List<Vector2> uvs);

public class UVMapper : MonoBehaviour {

    // The length of the adjacent side in a right angled triangle
    // with an angle of 45 deg and hypoteneuse of 1.
    // This is the same as if we were to compare the rotation of
    // the vector to some threshold.
    public const float adjacent = 0.70710678f;
    public const float sideThreshold = 0.1f;

    private readonly UVMapFn[] mapFunctions = { BoxMapping, BarkMapping };

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

    // a, b and c are vertices of a triangle, passed in CW order
    public static void BoxMapping(MaterialData mat, TriangleData triangle, List<Vector2> uvs) {
        float ax = triangle.x[0];
        float bx = triangle.x[1];
        float cx = triangle.x[2];
        float ay = triangle.y[0];
        float by = triangle.y[1];
        float cy = triangle.y[2];
        float az = triangle.z[0];
        float bz = triangle.z[1];
        float cz = triangle.z[2];
        Vector3 av = new Vector3(ax, ay, az);
        Vector3 bv = new Vector3(bx, by, bz);
        Vector3 cv = new Vector3(cx, cy, cz);
        Vector3 normal = Vector3.Cross(av - bv, av - cv).normalized;
        
        Vector2 a2 = Vector2.zero;
        Vector2 b2 = Vector2.zero;
        Vector2 c2 = Vector2.zero;

        // Sides are 0, top/bottom is 1
        int tile = 0;
        
        // normal points to either side
        if (WithinThreshold(normal, AXIS.Y, sideThreshold)) {

            // points to right or left
            // ignore x
            if (normal.z < adjacent && normal.z >= -adjacent) {
                // x > 0 ? right : left
                // Y axis is flipped in UV coords
                a2.x = (az + mat.halfSize.z) / mat.size.z;
                a2.y = (ay + mat.halfSize.y) / mat.size.y;
                b2.x = (bz + mat.halfSize.z) / mat.size.z;
                b2.y = (by + mat.halfSize.y) / mat.size.y;
                c2.x = (cz + mat.halfSize.z) / mat.size.z;
                c2.y = (cy + mat.halfSize.y) / mat.size.y;
            }

            // points forward or backward
            // ignore z
            if (normal.x < adjacent && normal.x >= -adjacent) {
                // z > 0 ? forward : backward
                a2.x = (ax + mat.halfSize.x) / mat.size.x;
                a2.y = (ay + mat.halfSize.y) / mat.size.y;
                b2.x = (bx + mat.halfSize.x) / mat.size.x;
                b2.y = (by + mat.halfSize.y) / mat.size.y;
                c2.x = (cx + mat.halfSize.x) / mat.size.x;
                c2.y = (cy + mat.halfSize.y) / mat.size.y;
            }

        } else {

            tile = 1;
            
            // normal points upwardish
            // ignore y
            a2.x = (ax + mat.halfSize.x) / mat.size.x;
            a2.y = (az + mat.halfSize.z) / mat.size.z;
            b2.x = (bx + mat.halfSize.x) / mat.size.x;
            b2.y = (bz + mat.halfSize.z) / mat.size.z;
            c2.x = (cx + mat.halfSize.x) / mat.size.x;
            c2.y = (cz + mat.halfSize.z) / mat.size.z;
        }
        
        uvs[triangle.indices[0]] = RemapUVCoordToTile(mat, a2, tile);
        uvs[triangle.indices[1]] = RemapUVCoordToTile(mat, b2, tile);
        uvs[triangle.indices[2]] = RemapUVCoordToTile(mat, c2, tile);
    }

    public static void BarkMapping(MaterialData mat, TriangleData triangle, List<Vector2> uvs) {
        
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
