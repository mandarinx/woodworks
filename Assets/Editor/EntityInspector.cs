using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Entity))]
public class EntityInspector : Editor {
    
    private MaterialIndex index;
    private UVMapper uvm;
    
    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("DEVELOPMENT", EditorStyles.boldLabel);
        
        index = (MaterialIndex)EditorGUILayout.ObjectField("Material Index", 
                                                           index, 
                                                           typeof(MaterialIndex), 
                                                           true);
        uvm = (UVMapper)EditorGUILayout.ObjectField("UV Mapper", 
                                                    uvm, 
                                                    typeof(UVMapper), 
                                                    true);
        if (index == null || uvm == null) {
            return;
        }
        
        Entity e = target as Entity;
        Mesh mesh = e.transform.GetComponent<MeshFilter>().sharedMesh;
        MaterialData md = MaterialIndex.GetMaterialData(index, e.materialID);
        
        if (GUILayout.Button("Box Mapping")) {
            UVMapper.SetUV(md, mesh, UVMapper.GetMapFunction(uvm, UVMapFnID.PROJECTION));
        }
        
        if (GUILayout.Button("Bark Mapping")) {
            UVMapper.SetUV(md, mesh, UVMapper.GetMapFunction(uvm, UVMapFnID.BARK));
        }
    }
}
