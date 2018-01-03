using System.Collections.Generic;
using UnityEngine;

public struct MaterialData {
    public UVMapFnID  mapFnID;
    public Vector2[]  tiles;
    public Vector2    barkThickness;
    
    // These ones are here only during dev. They belong to the
    // mesh data.
    public Vector3    size;
    public Vector3    halfSize;

    public MaterialData(UVMapFnID fn, Vector3 size, int numTiles, Vector2 bark) {
        this.size = size;
        mapFnID = fn;
        halfSize = size * 0.5f;
        tiles = new Vector2[numTiles * 2];
        barkThickness = new Vector2(bark.x, bark.y);
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

        // If the atlas consists of only equally sized tiles, this
        // would be very simple to replace with some math.
        // GetTileSize(MaterialData) => returns the tile size for a given atlas
        // GetTileOffset(MaterialData, tile) => multiplies the tile index by size, modulo
        // to width, and return the offset.
        // With this, each MaterialData could have an integer to point
        // to the tile for each of the sides, with/without bark. 

        Vector2 tilesize   = new Vector2(0.25f, 1f);
        Vector2 sideBark   = new Vector2(0.00f, 0f);
        Vector2 sideWOBark = new Vector2(0.25f, 0f);
        Vector2 edgeBark   = new Vector2(0.50f, 0f);
        Vector2 edgeWOBark = new Vector2(0.75f, 0f);
        
        MaterialData md1 = new MaterialData(UVMapFnID.PROJECTION, 
                                            new Vector3(1, 2, 1), 
                                            4,
                                            Vector2.zero);
        MaterialData.SetTile(md1, 0, sideBark,   tilesize);
        MaterialData.SetTile(md1, 1, sideWOBark, tilesize);
        MaterialData.SetTile(md1, 2, edgeBark,   tilesize);
        MaterialData.SetTile(md1, 3, edgeWOBark, tilesize);
        materials.Add(MaterialID.WOOD, md1);
        
        MaterialData md2 = new MaterialData(UVMapFnID.BARK, 
                                            new Vector3(1, 2, 1), 
                                            4,
                                            new Vector2(0.0625f, 0f));
        MaterialData.SetTile(md2, 0, sideBark,   tilesize);
        MaterialData.SetTile(md2, 1, sideWOBark, tilesize);
        MaterialData.SetTile(md2, 2, edgeBark,   tilesize);
        MaterialData.SetTile(md2, 3, edgeWOBark, tilesize);
        materials.Add(MaterialID.WOOD_BARK, md2);
    }

    public static MaterialData GetMaterialData(MaterialIndex index, MaterialID id) {
        return index.materials[id];
    }
}
