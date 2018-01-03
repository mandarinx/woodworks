using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MeshSlicer))]
public class MeshSlicerInspector : Editor {
    
    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        if (GUILayout.Button("Slice")) {
            MeshSlicer slicer = target as MeshSlicer;
            slicer.Slice(slicer.entity);
        }
    }
}
