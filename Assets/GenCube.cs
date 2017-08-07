using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
public class GenCube : MonoBehaviour {

    [Range(0.5f, 5f)]
    public float width = 0.5f;
    [Range(0.5f, 5f)]
    public float height = 0.5f;
    [Range(0.5f, 5f)]
    public float length = 0.5f;
    
    private MeshFilter mf;
    
    void Awake() {
        mf = GetComponent<MeshFilter>();
    }

    void OnEnable() {
        Mesh mesh = mf.sharedMesh;
        mesh.vertices = new [] {
            // top
            new Vector3(width * -.5f, height *  .5f, length * -.5f), // left   top    back 
            new Vector3(width *  .5f, height *  .5f, length *  .5f), // right  top    front
            new Vector3(width *  .5f, height *  .5f, length * -.5f), // right  top    back

            new Vector3(width * -.5f, height *  .5f, length * -.5f), // left   top    back 
            new Vector3(width * -.5f, height *  .5f, length *  .5f), // left   top    front
            new Vector3(width *  .5f, height *  .5f, length *  .5f), // right  top    front

            // back
            new Vector3(width * -.5f, height *  .5f, length * -.5f), // left   top    back 
            new Vector3(width *  .5f, height *  .5f, length * -.5f), // right  top    back
            new Vector3(width *  .5f, height * -.5f, length * -.5f), // right  bottom back 

            new Vector3(width * -.5f, height *  .5f, length * -.5f), // left   top    back 
            new Vector3(width *  .5f, height * -.5f, length * -.5f), // right  bottom back 
            new Vector3(width * -.5f, height * -.5f, length * -.5f), // left   bottom back 

            // left
            new Vector3(width * -.5f, height *  .5f, length * -.5f), // left   top    back 
            new Vector3(width * -.5f, height * -.5f, length * -.5f), // left   bottom back 
            new Vector3(width * -.5f, height * -.5f, length *  .5f), // left   bottom front

            new Vector3(width * -.5f, height *  .5f, length * -.5f), // left   top    back 
            new Vector3(width * -.5f, height * -.5f, length *  .5f), // left   bottom front
            new Vector3(width * -.5f, height *  .5f, length *  .5f), // left   top    front

            // front
            new Vector3(width *  .5f, height * -.5f, length *  .5f), // right  bottom front
            new Vector3(width *  .5f, height *  .5f, length *  .5f), // right  top    front
            new Vector3(width * -.5f, height *  .5f, length *  .5f), // left   top    front

            new Vector3(width *  .5f, height * -.5f, length *  .5f), // right  bottom front
            new Vector3(width * -.5f, height *  .5f, length *  .5f), // left   top    front
            new Vector3(width * -.5f, height * -.5f, length *  .5f), // left   bottom front

            // right
            new Vector3(width *  .5f, height * -.5f, length *  .5f), // right  bottom front
            new Vector3(width *  .5f, height *  .5f, length * -.5f), // right  top    back
            new Vector3(width *  .5f, height *  .5f, length *  .5f), // right  top    front

            new Vector3(width *  .5f, height * -.5f, length *  .5f), // right  bottom front
            new Vector3(width *  .5f, height * -.5f, length * -.5f), // right  bottom back 
            new Vector3(width *  .5f, height *  .5f, length * -.5f), // right  top    back

            // bottom
            new Vector3(width *  .5f, height * -.5f, length *  .5f), // right  bottom front
            new Vector3(width * -.5f, height * -.5f, length *  .5f), // left   bottom front
            new Vector3(width * -.5f, height * -.5f, length * -.5f), // left   bottom back 

            new Vector3(width *  .5f, height * -.5f, length *  .5f), // right  bottom front
            new Vector3(width * -.5f, height * -.5f, length * -.5f), // left   bottom back 
            new Vector3(width *  .5f, height * -.5f, length * -.5f), // right  bottom back 
        };
        mesh.triangles = new [] {
            // top quad
            0, 1, 2,
            3, 4, 5,
            // bottom quad
            6, 7, 8,
            9, 10, 11,
            // left quad
            12, 13, 14,
            15, 16, 17,
            // right quad
            18, 19, 20,
            21, 22, 23,
            // front quad
            24, 25, 26,
            27, 28, 29,
            // back quad
            30, 31, 32,
            33, 34, 35,
        };
    }
}
