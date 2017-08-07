using UnityEngine;

public class Entity : MonoBehaviour {

    public MaterialID materialID;

    private void OnDrawGizmos() {
//        Gizmos.color = Color.magenta;
//        Gizmos.DrawWireCube(transform.position, originalSize);
//
//        Gizmos.color = GetColor(transform.forward);
//        Gizmos.DrawRay(transform.position, transform.forward);
        
        Mesh mesh = GetComponent<MeshFilter>().sharedMesh;
        
        for (int i = 0; i < mesh.triangles.Length; i+=3) {
            Vector3 va = mesh.vertices[mesh.triangles[i]];
            Vector3 vb = mesh.vertices[mesh.triangles[i + 1]];
            Vector3 vc = mesh.vertices[mesh.triangles[i + 2]];
            Vector3 normal = Vector3.Cross(va - vb, va - vc).normalized;
            Gizmos.color = GetColor(normal);
            Gizmos.DrawRay(transform.position + (va + vb + vc) / 3, normal * 0.3f);
        }
    }

    private Color GetColor(Vector3 n) {
        // normal points to either side
        if (UVMapper.WithinThreshold(n, AXIS.Y, UVMapper.sideThreshold)) {

            // points to right or left
            if (n.z < UVMapper.adjacent && n.z >= -UVMapper.adjacent) {
                return n.x > 0 ? Color.blue : Color.black;
            }

            // points forward or backward
            if (n.x < UVMapper.adjacent && n.x >= -UVMapper.adjacent) {
                return n.z > 0 ? Color.green : Color.cyan;
            }
        }
        
        // upwardish
        return n.y >= UVMapper.sideThreshold
            ? Color.magenta
            : Color.yellow;
    }

}
