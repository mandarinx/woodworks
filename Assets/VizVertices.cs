using UnityEngine;

public class VizVertices : MonoBehaviour {
    
    [SerializeField]
    private float sphereSize = 0.1f;

    [SerializeField]
    private int selectedTri;
    
    private void OnDrawGizmos() {
        Mesh mesh = GetComponent<MeshFilter>()?.sharedMesh;
        if (mesh == null) {
            return;
        }

        selectedTri = Mathf.Clamp(selectedTri, 0, mesh.vertices.Length / 3);
        int tri = 0;
        for (int i = 0; i < mesh.vertices.Length; ++i) {
            tri += i > 0 && i % 3 == 0 ? 1 : 0;
            int ti = tri * 3;
            bool selected = selectedTri == tri && i >= ti && i < ti + 3;
            Gizmos.color = selected ? Color.green : Color.white;
            Gizmos.DrawSphere(transform.position + mesh.vertices[i], sphereSize * (selected ? 2f : 1f));
        }
    }
}
