using UnityEngine;

public enum EEntityType 
{
    Neutral = 0,
    PlayerUnit = 1,
    Enemy = 2
}

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

[System.Serializable]
public class SpawnData
{
    public Vector2Int gridPos;
    public int entityId;
    public EEntityType entityType;
}