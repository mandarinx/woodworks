using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.Rendering;

[ExecuteInEditMode]
public class MeshMirroring : MonoBehaviour {

    public MeshFilter              meshSource;
    public MeshFilter              meshSliced;
    public MeshFilter              meshMirrored;
    public Transform               slicePlane;
    
    // Front is the piece cut off
    private readonly List<Vector3> bufferFront = new List<Vector3>();
    // Back is the mesh you cut in
    private readonly List<Vector3> bufferBack = new List<Vector3>();
    private List<Vector3>          vertBufferFront = new List<Vector3>();
    private List<Vector3>          vertBufferBack = new List<Vector3>();
    private readonly List<Vector3> mirroredVertices = new List<Vector3>();
    private readonly Vector3[]     verticesSutherland = new Vector3[3];
    
    private readonly List<Vector3> verticesSource = new List<Vector3>();
    private readonly List<int> trianglesSource = new List<int>();
    private readonly List<int> trianglesSliced = new List<int>(1024*1024);
    private readonly List<int> trianglesMirrored = new List<int>(1024*1024);
    
    private void OnEnable() {
        verticesSource.AddRange(meshSource.sharedMesh.vertices);
        trianglesSource.AddRange(meshSource.sharedMesh.triangles);

        if (meshSliced.sharedMesh == null) {
            meshSliced.sharedMesh = new Mesh();
        }
        
        meshSliced.sharedMesh.MarkDynamic();
        meshSliced.sharedMesh.indexFormat = IndexFormat.UInt32;

        if (meshMirrored.sharedMesh == null) {
            meshMirrored.sharedMesh = new Mesh();
        }
        
        meshMirrored.sharedMesh.MarkDynamic();
        meshMirrored.sharedMesh.indexFormat = IndexFormat.UInt32;
        
        meshSource.sharedMesh.indexFormat = IndexFormat.UInt32;
    }

    private void Update() {
        bufferBack.Clear();
        bufferFront.Clear();
        
        // Slice each triangle
        for (int i = 0; i < trianglesSource.Count; i += 3) {
            int t1 = trianglesSource[i + 0];
            int t2 = trianglesSource[i + 1];
            int t3 = trianglesSource[i + 2];
            verticesSutherland[0] = verticesSource[t1];
            verticesSutherland[1] = verticesSource[t2];
            verticesSutherland[2] = verticesSource[t3];

            vertBufferBack.Clear();
            vertBufferFront.Clear();
            
            // Create an override for the SutherlandHodgman that optionally accepts
            // a front buffer
            Methods.SutherlandHodgman(
                verticesSutherland, 
                slicePlane.forward, 
                slicePlane.position - meshSource.transform.position, 
                ref vertBufferFront, 
                ref vertBufferBack);
            Methods.CopyVertBuffer(vertBufferBack, bufferBack);
            Methods.CopyVertBuffer(vertBufferFront, bufferFront);
        }
        
        mirroredVertices.Clear();
        
        // Mirror the back buffer using the slice plane
        for (int i = 0; i < bufferBack.Count; i += 3) {
            mirroredVertices.Add(bufferBack[i + 2] + slicePlane.forward * Methods.DistToPlane(bufferBack[i + 2], slicePlane.position, slicePlane.forward) * -2f);
            mirroredVertices.Add(bufferBack[i + 1] + slicePlane.forward * Methods.DistToPlane(bufferBack[i + 1], slicePlane.position, slicePlane.forward) * -2f);
            mirroredVertices.Add(bufferBack[i + 0] + slicePlane.forward * Methods.DistToPlane(bufferBack[i + 0], slicePlane.position, slicePlane.forward) * -2f);
        }
        
        if (meshSliced != null) {
            RecreateMesh(meshSliced.sharedMesh, bufferBack, trianglesSliced);
        }
        
        if (meshMirrored != null) {
            RecreateMesh(meshMirrored.sharedMesh, mirroredVertices, trianglesMirrored);
        }
    }

    private static void RecreateMesh(Mesh mesh, List<Vector3> verts, List<int> tris) {
        tris.Clear();
        for (int i = 0; i < verts.Count; ++i) {
            tris.Add(i);
        }
        
        mesh.Clear();
        mesh.SetVertices(verts);
        mesh.SetTriangles(tris, 0);
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
    }

    public void Weld() {
        verticesSource.Clear();
        verticesSource.AddRange(bufferBack);
        verticesSource.AddRange(mirroredVertices);
        
        RecreateMesh(meshSource.sharedMesh, verticesSource, trianglesSource);
    }

    public void ClearBuffers() {
        meshSource.GetComponent<GenCube>().Reset();

        bufferBack.Clear();
        trianglesSliced.Clear();
        verticesSource.Clear();
        trianglesSource.Clear();
        mirroredVertices.Clear();
        trianglesMirrored.Clear();
        
        meshSliced.sharedMesh.Clear();
        meshSliced.sharedMesh.SetVertices(null);
        meshSliced.sharedMesh.SetTriangles(new int[0], 0);
        meshSliced.sharedMesh = null;

        meshMirrored.sharedMesh.Clear();
        meshMirrored.sharedMesh.SetVertices(null);
        meshMirrored.sharedMesh.SetTriangles(new int[0], 0);
        meshMirrored.sharedMesh = null;
        
        OnEnable();
    }

//    private void OnDrawGizmos() {
//        Gizmos.color = Color.red;
//        for (int i = 0; i < bufferBack.Count; ++i) {
//            Gizmos.DrawSphere(transform.position + bufferBack[i], 0.01f);
//        }
//        Gizmos.color = Color.blue;
//        for (int i = 0; i < bufferFront.Count; ++i) {
//            Gizmos.DrawSphere(transform.position + bufferFront[i], 0.01f);
//        }
//    }
}
