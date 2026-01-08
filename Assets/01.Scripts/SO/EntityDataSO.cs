using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitDataSO", menuName = "Scriptable Objects/UnitDataSO")]
public class EntityDataSO : ScriptableObject
{
    public string unitName;
    public EEntityType unitType;
    public Entity unitPrefab;
    public int unitId;
    public int unitHP;
    public int unitMP;
    public int unitAttack;
    public int unitDefense;
    public int unitAP;  //행동력 > 이동 한칸에 AP1 소모
    public int unitAttackRange;

    public List<SkillSO> skills = new List<SkillSO>();
}
