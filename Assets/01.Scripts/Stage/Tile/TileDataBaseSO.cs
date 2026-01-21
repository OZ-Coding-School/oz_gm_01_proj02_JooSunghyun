using UnityEngine;
public enum ETileType
{
    Plane,
    Desert,
    Snow
}

[System.Serializable]
public class TileSpawnData
{
    public ETileType type;
    public int tileId;
    public int tileHeight;
    public int tilePosX;
    public int tilePosZ;
}

[CreateAssetMenu(fileName = "TileDataBaseSO", menuName = "Scriptable Objects/TileDataBaseSO")]
public class TileDataBaseSO : ScriptableObject
{
    //타일 종류/프리팹
    [System.Serializable]
    public struct TileEntry 
    {
        public ETileType type;
        public int tileId;
        public TileBase prefab;
    }

    public TileEntry[] entries;

    public TileBase GetPrefab(int id) 
    {
        foreach (var entry in entries) 
        {
            if (entry.tileId == id) return entry.prefab;
        }
        return null;
    }
}
