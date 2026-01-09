using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitDataSO", menuName = "Scriptable Objects/UnitDataSO")]
public class EntityDataSO : ScriptableObject
{
    public string unitName;
    public EEntityType unitType;
    public Entity unitPrefab;
    public int unitId;
    public int unitAP;  //행동력 > 이동 한칸에 AP1 소모
    public int unitAttackRange;
    public float unitHP;
    public float unitMP;
    public float unitAttack;
    public float unitDefense;

    public List<SkillSO> skills = new List<SkillSO>();
}
