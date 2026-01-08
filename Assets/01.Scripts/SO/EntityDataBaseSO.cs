using UnityEngine;

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
