using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MeshMirroring))]
public class MeshMirroringInspector : Editor {

    public override void OnInspectorGUI() {
        serializedObject.Update();
        DrawDefaultInspector();

        if (GUILayout.Button("Weld")) {
            (target as MeshMirroring)?.Weld();
        }

        if (GUILayout.Button("ClearBuffers")) {
            (target as MeshMirroring)?.ClearBuffers();
        }
        
        serializedObject.ApplyModifiedProperties();
    }
}
