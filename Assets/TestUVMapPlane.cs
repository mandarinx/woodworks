using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class TestUVMapPlane : MonoBehaviour {

    [Range(0, 1)]
    public int tile;
    public Vector2[] uvMapping;
    
    private static float adjacent = 0.70710678f;

    private void OnEnable() {
        Mesh mesh = new Mesh();
        mesh.name = "Plane";
        
        mesh.vertices = new [] {
            new Vector3(-.5f,  0.5f, -.5f), // left  back 
            new Vector3( .5f,  0.5f, -.5f), // right back
            new Vector3( .5f,  0.5f,  .5f), // right front
            new Vector3(-.5f,  0.5f,  .5f), // left  front

            new Vector3(-.5f,  0.5f, -.5f), // left  back 
            new Vector3( .5f,  0.5f, -.5f), // right back
            new Vector3(-.5f, -0.5f, -.5f), // left  bottom back 
            new Vector3( .5f, -0.5f, -.5f), // right bottom back
            
            new Vector3( .5f,  0.5f, -.5f), // right back
            new Vector3( .5f,  0.5f,  .5f), // right front
            new Vector3( .5f, -0.5f,  .5f), // right bottom front 
            new Vector3( .5f, -0.5f, -.5f), // right bottom back
        };
        mesh.triangles = new [] {
            // top
            0, 2, 1,
            0, 3, 2,
            // back
            4, 5, 7,
            4, 7, 6,
            // right
            8, 9, 10,
            8, 10, 11,
        };
        
        // for viz here, not needed in UVMapper
        mesh.RecalculateNormals();
        Vector2[] uvs = new Vector2[mesh.vertices.Length];
        
        for (int i = 0; i < mesh.triangles.Length; i+=3) {
            int t1 = mesh.triangles[i];
            int t2 = mesh.triangles[i + 1];
            int t3 = mesh.triangles[i + 2];
            Vector3 va = mesh.vertices[t1];
            Vector3 vb = mesh.vertices[t2];
            Vector3 vc = mesh.vertices[t3];
            Vector3 normal = Vector3.Cross(va - vb, va - vc).normalized;

            int t = 0;
            Vector2 offset = uvMapping[t * 2];
            Vector2 size = uvMapping[t * 2 + 1];

            // to the sides
            if (normal.y < 0.1f && normal.y > -0.1f) {
                
                // right/left
                if (normal.z < adjacent && normal.z >= -adjacent) {
                    Vector2 uva = new Vector2((va.z + 0.5f) / 1, (va.y + 0.5f) / 1);
                    uvs[t1] = new Vector2(Map(    uva.x, 0, 1, offset.x, offset.x + size.x),
                                          Map(1 - uva.y, 0, 1, offset.y, offset.y + size.y));
                    Vector2 uvb = new Vector2((vb.z + 0.5f) / 1, (vb.y + 0.5f) / 1);
                    uvs[t2] = new Vector2(Map(    uvb.x, 0, 1, offset.x, offset.x + size.x),
                                          Map(1 - uvb.y, 0, 1, offset.y, offset.y + size.y));
                    Vector2 uvc = new Vector2((vc.z + 0.5f) / 1, (vc.y + 0.5f) / 1);
                    uvs[t3] = new Vector2(Map(    uvc.x, 0, 1, offset.x, offset.x + size.x),
                                          Map(1 - uvc.y, 0, 1, offset.y, offset.y + size.y));
                }

                // forward/back
                if (normal.x < adjacent && normal.x >= -adjacent) {
//                    Debug.Log("f/b "+t1+", "+t2+", "+t3);
                    Vector2 uva = new Vector2((va.x + 0.5f) / 1, (va.y + 0.5f) / 1);
                    uvs[t1] = new Vector2(Map(    uva.x, 0, 1, offset.x, offset.x + size.x),
                                          Map(1 - uva.y, 0, 1, offset.y, offset.y + size.y));
//                    Debug.Log("uva x "+uva.x+" y: "+uva.y+" => "+uvs[t1].x+", "+uvs[t1].y);
                    Vector2 uvb = new Vector2((vb.x + 0.5f) / 1, (vb.y + 0.5f) / 1);
                    uvs[t2] = new Vector2(Map(    uvb.x, 0, 1, offset.x, offset.x + size.x),
                                          Map(1 - uvb.y, 0, 1, offset.y, offset.y + size.y));
//                    Debug.Log("uvb x "+uvb.x+" y: "+uvb.y+" => "+uvs[t2].x+", "+uvs[t2].y);
                    Vector2 uvc = new Vector2((vc.x + 0.5f) / 1, (vc.y + 0.5f) / 1);
                    uvs[t3] = new Vector2(Map(    uvc.x, 0, 1, offset.x, offset.x + size.x),
                                          Map(1 - uvc.y, 0, 1, offset.y, offset.y + size.y));
//                    Debug.Log("uvc x "+uvc.x+" y: "+uvc.y+" => "+uvs[t3].x+", "+uvs[t3].y);
                }

            } else {
//                Debug.Log("u/d "+t1+", "+t2+", "+t3);
                Vector2 uva = new Vector2((va.x + 0.5f) / 1, (va.z + 0.5f) / 1);
                uvs[t1] = new Vector2(Map(    uva.x, 0, 1, offset.x, offset.x + size.x),
                                      Map(1 - uva.y, 0, 1, offset.y, offset.y + size.y));
//                Debug.Log("uva x "+uva.x+" y: "+uva.y+" => "+uvs[t1].x+", "+uvs[t1].y);
                Vector2 uvb = new Vector2((vb.x + 0.5f) / 1, (vb.z + 0.5f) / 1);
                uvs[t2] = new Vector2(Map(    uvb.x, 0, 1, offset.x, offset.x + size.x),
                                      Map(1 - uvb.y, 0, 1, offset.y, offset.y + size.y));
//                Debug.Log("uvb x "+uvb.x+" y: "+uvb.y+" => "+uvs[t2].x+", "+uvs[t2].y);
                Vector2 uvc = new Vector2((vc.x + 0.5f) / 1, (vc.z + 0.5f) / 1);
                uvs[t3] = new Vector2(Map(    uvc.x, 0, 1, offset.x, offset.x + size.x),
                                      Map(1 - uvc.y, 0, 1, offset.y, offset.y + size.y));
//                Debug.Log("uvc x "+uvc.x+" y: "+uvc.y+" => "+uvs[t3].x+", "+uvs[t3].y);
            }
        }
        
        mesh.SetUVs(0, new List<Vector2>(uvs));
        
        GetComponent<MeshFilter>().sharedMesh = mesh;
    }

//    void Update() {
//        Mesh mesh = GetComponent<MeshFilter>().sharedMesh;
//
//        Vector2 offset = uvMapping[tile * 2];
//        Vector2 dim = uvMapping[tile * 2 + 1];
//        
//        List<Vector2> uvs = new List<Vector2>(mesh.vertices.Length);
//
//        for (int i = 0; i < mesh.vertices.Length; ++i) {
//            float x = (mesh.vertices[i].x + 0.5f) / 1;
//            float y = (mesh.vertices[i].z + 0.5f) / 1;
//            uvs.Add(new Vector2(Map(    x, 0, 1, offset.x, offset.x + dim.x),
//                                Map(1 - y, 0, 1, offset.y, offset.y + dim.y)));
//        }
//        
//        mesh.SetUVs(0, uvs);
//    }
    
    private float Map(float x, float in_min, float in_max, float out_min, float out_max) {
        return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
    }

    private void OnDrawGizmos() {
        Mesh mesh = GetComponent<MeshFilter>().sharedMesh;
        for (int i = 0; i < mesh.vertices.Length; ++i) {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position + mesh.vertices[i], mesh.normals[i]);
        }
    }
}
