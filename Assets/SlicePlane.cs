using UnityEngine;

public class SlicePlane : MonoBehaviour {
    
    void OnDrawGizmos() {
//        Gizmos.color = Color.magenta;
//        Gizmos.DrawSphere(transform.position - transform.right * 0.5f, 0.1f);
        Gizmos.color = UVMapper.IsAlignedY(transform.forward, 85f) ? Color.cyan : Color.white;
        Gizmos.DrawRay(transform.position, transform.forward);
        
    }
}
