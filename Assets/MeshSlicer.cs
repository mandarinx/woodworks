﻿using UnityEngine;
using System.Collections.Generic;
using MIConvexHull;

public class MeshSlicer : MonoBehaviour {

    public Entity                 entity;
    public Transform              sliceplane;
    public MaterialIndex          matIndex;
    public UVMapper               uvMapper;
    
    // Note that the sliceplane needs to always be facing away from the player
    // for the sorting of front and back to always be correct.
    // Front is the piece cut off
    private List<Vector3>         bufferFront = new List<Vector3>();
    // Back is the mesh you cut in
    private List<Vector3>         bufferBack = new List<Vector3>();
    private List<Vector3>         vertBufferFront = new List<Vector3>();
    private List<Vector3>         vertBufferBack = new List<Vector3>();
    private readonly Vector3[]    edges = new Vector3[3];
    
    public void Slice(Entity entity) {
        bufferBack.Clear();
        bufferFront.Clear();

        MeshFilter mesh = entity.GetComponent<MeshFilter>();
        
        // Slice each triangle
        for (int i = 0; i < mesh.sharedMesh.triangles.Length; i += 3) {
            int t1 = mesh.sharedMesh.triangles[i];
            int t2 = mesh.sharedMesh.triangles[i + 1];
            int t3 = mesh.sharedMesh.triangles[i + 2];
            edges[0] = mesh.sharedMesh.vertices[t1];
            edges[1] = mesh.sharedMesh.vertices[t2];
            edges[2] = mesh.sharedMesh.vertices[t3];
            vertBufferBack.Clear();
            vertBufferFront.Clear();
            Methods.SutherlandHodgman(
                edges, 
                -sliceplane.forward, 
                sliceplane.position - mesh.transform.position, 
                ref vertBufferFront, 
                ref vertBufferBack);
            Methods.CopyVertBuffer(vertBufferBack, bufferBack);
            Methods.CopyVertBuffer(vertBufferFront, bufferFront);
        }

        UpdateMesh(bufferBack, mesh.sharedMesh, entity.materialID, uvMapper);
        mesh.GetComponent<MeshCollider>().enabled = false;
        mesh.GetComponent<MeshCollider>().enabled = true;
        
        Mesh frontMesh = new Mesh();
        UpdateMesh(bufferFront, frontMesh, entity.materialID, uvMapper);
        
        GameObject front = new GameObject();
        front.transform.position = mesh.transform.position;
        front.AddComponent<MeshFilter>().sharedMesh = frontMesh;
        front.AddComponent<MeshRenderer>().sharedMaterial = mesh.transform.GetComponent<MeshRenderer>().sharedMaterial;
        front.AddComponent<MeshCollider>().convex = true;
        Rigidbody rb = front.AddComponent<Rigidbody>();
        rb.AddForceAtPosition(
            -sliceplane.forward * 200f,
            sliceplane.position - sliceplane.right * 0.5f
            );
    }

    private void UpdateMesh(List<Vector3> input, Mesh mesh, MaterialID matid, UVMapper uvMapper) {
        Vertex3[] inputVtx3 = new Vertex3[input.Count];
        CastToVertex3(input, inputVtx3);

        ConvexHull<Vertex3, Face3> hull = ConvexHull.Create<Vertex3, Face3>(inputVtx3, 0.0001);

//        List<Vertex3> hullVtx3 = new List<Vertex3>(hull.Points);
        List<Vertex3> hullVtx3 = new List<Vertex3>();
        List<Face3> faces = new List<Face3>(hull.Faces);
        int[] indices = new int[faces.Count * 3];

        int n = 0;
        for (int i = 0; i < faces.Count; ++i) {
        
            // Sometime in the future, I'd like each side of the log
            // to share vertices, and only separate them along the 
            // cardinal edges.
            
            // This is how we do it when we want to separate each
            // triangle. We create a vertex for each point of each
            // triangle.
            hullVtx3.Add(faces[i].Vertices[0]);
            indices[n++] = hullVtx3.Count - 1;
            hullVtx3.Add(faces[i].Vertices[1]);
            indices[n++] = hullVtx3.Count - 1;
            hullVtx3.Add(faces[i].Vertices[2]);
            indices[n++] = hullVtx3.Count - 1;

            // This is the way to do it when you want to share
            // vertices between triangles. That's not going to
            // work with texture atlassing.            
//            indices[n++] = hullVtx3.IndexOf(faces[i].Vertices[0]);
//            indices[n++] = hullVtx3.IndexOf(faces[i].Vertices[1]);
//            indices[n++] = hullVtx3.IndexOf(faces[i].Vertices[2]);
        }

        Vector3[] vertices = new Vector3[hullVtx3.Count];
        CastToVector3(hullVtx3, vertices);

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.SetIndices(indices, MeshTopology.Triangles, 0);

        MaterialData md = MaterialIndex.GetMaterialData(matIndex, matid);
        UVMapFn mapFn = UVMapper.GetMapFunction(uvMapper, md.mapFnID);
        UVMapper.SetUV(md, mesh, mapFn);

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }
    
    private void CastToVertex3(List<Vector3> input, Vertex3[] output) {
        for (int i = 0; i < output.Length; ++i) {
            Vector3 v = input[i];
            output[i] = new Vertex3(v.x, v.y, v.z);
        }
    }
    
    private void CastToVector3(List<Vertex3> input, Vector3[] output) {
        for (int i = 0; i < output.Length; ++i) {
            output[i] = input[i].ToVector3();
        }
    }

    private void OnDrawGizmos() {
//        Vector3 redOffset = Vector3.down;
//        Gizmos.color = Color.red;
//        for (int i = 0; i < bufferBack.Count; ++i) {
//            Gizmos.DrawSphere(bufferBack[i] + redOffset, 0.02f);
//        }
//        
//        Vector3 blueOffset = Vector3.up;
//        Gizmos.color = Color.blue;
//        for (int i = 0; i < bufferFront.Count; ++i) {
//            Gizmos.DrawSphere(bufferFront[i] + blueOffset, 0.02f);
//        }
    }
}
