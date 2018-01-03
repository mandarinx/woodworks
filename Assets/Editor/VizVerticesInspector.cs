using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(VizVertices))]
public class VizVerticesInspector : Editor {

    public override void OnInspectorGUI() {
        serializedObject.Update();

        EditorGUILayout.Slider(serializedObject.FindProperty("sphereSize"), 0, 1f);

        int vertices = (target as VizVertices).GetComponent<MeshFilter>().sharedMesh.vertices.Length / 3;
        SerializedProperty selected = serializedObject.FindProperty("selectedTri");
        selected.intValue = (int)EditorGUILayout.Slider("Selected Triangle", selected.intValue, 0, vertices);
        
        serializedObject.ApplyModifiedProperties();
    }
}
