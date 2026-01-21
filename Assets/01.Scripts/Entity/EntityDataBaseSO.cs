using UnityEngine;

public enum EEntityType
{
    Neutral = 0,
    PlayerUnit = 1,
    Enemy = 2
}

[System.Serializable]
public class SpawnData
{
    public Vector2Int gridPos;
    public int entityId;
    public EEntityType entityType;
}

[CreateAssetMenu(fileName = "EntityDataBaseSO", menuName = "Scriptable Objects/EntityDataBaseSO")]
public class EntityDataBaseSO : ScriptableObject
{
    public EntityDataSO[] entities;

    public Entity GetPrefab(int id)
    {
        foreach (var entry in entities)
        {
            if (entry.unitId == id) return entry.unitPrefab;
        }
        return null;
    }

    public EntityDataSO GetDataSO(int id)
    {
        foreach (var entry in entities)
        {
            if (entry.unitId == id) return entry;
        }
        return null;
    }
}
