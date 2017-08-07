using System.Collections.Generic;
using UnityEngine;

public struct MaterialData {
    public UVMapFnID  mapFnID;
    public Vector3    size;
    public Vector2[]  tiles;
    public Vector3    halfSize;

    public MaterialData(UVMapFnID fn, Vector3 size, int numTiles) {
        this.size = size;
        mapFnID = fn;
        halfSize = size * 0.5f;
        tiles = new Vector2[numTiles * 2];
    }

    public static void SetTile(MaterialData md, int n, Vector2 offset, Vector2 size) {
        md.tiles[n * 2] = offset;
        md.tiles[n * 2 + 1] = size;
    }
}

public enum MaterialID {
    NONE = 0,
    WOOD = 1,
    WOOD_BARK = 2,
}

public struct MaterialIDComparer : IEqualityComparer<MaterialID> {
    public bool Equals(MaterialID a, MaterialID b) {
        return (int)a == (int)b;
    }

    public int GetHashCode(MaterialID obj) {
        return (int)obj;
    }
}

[ExecuteInEditMode]
public class MaterialIndex : MonoBehaviour {

    private Dictionary<MaterialID, MaterialData> materials;

    private void OnEnable() {
        materials = new Dictionary<MaterialID, MaterialData>(new MaterialIDComparer());
        
        MaterialData md1 = new MaterialData(UVMapFnID.BOX, new Vector3(1, 2, 1), 2);
        MaterialData.SetTile(md1, 0, new Vector2(0f,   0f), new Vector2(0.5f, 1f));
        MaterialData.SetTile(md1, 1, new Vector2(0.5f, 0f), new Vector2(0.5f, 1f));
        materials.Add(MaterialID.WOOD, md1);
        
        MaterialData md2 = new MaterialData(UVMapFnID.BOX, new Vector3(1, 2, 1), 2);
        MaterialData.SetTile(md2, 0, new Vector2(0f,   0f), new Vector2(0.5f, 1f));
        MaterialData.SetTile(md2, 1, new Vector2(0.5f, 0f), new Vector2(0.5f, 1f));
        materials.Add(MaterialID.WOOD_BARK, md2);
    }

    public static MaterialData GetMaterialData(MaterialIndex index, MaterialID id) {
        return index.materials[id];
    }
}
