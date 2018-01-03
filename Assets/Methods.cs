using UnityEngine;
using System.Collections.Generic;

public static class Methods {

    public static void SutherlandHodgman(Vector3[] verts, Vector3 normal, Vector3 q, ref List<Vector3> front, ref List<Vector3> back) {
        Vector3 previous = verts[verts.Length - 1];
        float distPrevious = DistToPlane(previous, q, normal);
        
        for (int i = 0; i < verts.Length; ++i) {
            Vector3 current = verts[i];
            float distCurrent= DistToPlane(current, q, normal);
            
            if (InFrontOfPlane(distCurrent)) {
                if (AtBackOfPlane(distPrevious)) {
                    Vector3 vi = Vector3.Lerp(current, previous, distCurrent/ (distCurrent- distPrevious));
                    front.Add(vi);
                    back.Add(vi);
                }
                front.Add(current);
                
            } else if (AtBackOfPlane(distCurrent)) {
                if (InFrontOfPlane(distPrevious)) {
                    Vector3 vi = Vector3.Lerp(previous, current, distPrevious / (distPrevious - distCurrent));
                    front.Add(vi);
                    back.Add(vi);
                } else if (OnPlane(distPrevious)) {
                    back.Add(previous);
                }
                back.Add(current);
                
            } else {
                front.Add(current);
//                if (OnPlane(distPrevious)) {
//                    back.Add(current);
//                }
            }
            
            previous = current;
            distPrevious = distCurrent;
        }
    }

    // Get the distance from a point to a plane
    // P is the point
    // Q is a point on the plane
    // N is the normal of the plane
    public static float DistToPlane(Vector3 p, Vector3 q, Vector3 n) {
        return Vector3.Dot(p - q, n) / n.magnitude;
    }

    public static bool InFrontOfPlane(float dist) {
        return dist > float.Epsilon;
    }

    public static bool AtBackOfPlane(float dist) {
        return dist < float.Epsilon;
    }

    public static bool OnPlane(float dist) {
        return !InFrontOfPlane(dist) && !AtBackOfPlane(dist);
    }

    // Copy the vertex buffer from a Sutherland Hodgman slice into a vertex buffer.
    // Sutherland Hodgman slices a triange into two parts. Depending on the slice angle, 
    // the result can be either two lists of three vertices, or one list of three verts and
    // another of four verts. The list of four verts needs to be copied over as two triangles.
    public static void CopyVertBuffer(List<Vector3> vertBuffer, List<Vector3> targetVerts) {
        int vbc = vertBuffer.Count;
        if (vbc == 4) {
            targetVerts.Add(vertBuffer[0]);
            targetVerts.Add(vertBuffer[1]);
            targetVerts.Add(vertBuffer[2]);
            targetVerts.Add(vertBuffer[0]);
            targetVerts.Add(vertBuffer[2]);
            targetVerts.Add(vertBuffer[3]);
            return;
        }
        for (int j = 0; j < vbc; ++j) {
            targetVerts.Add(vertBuffer[j]);
        }
    }
    
}
