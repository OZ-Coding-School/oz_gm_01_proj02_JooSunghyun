using UnityEngine;
using System;

[CreateAssetMenu(fileName = "SkillSO", menuName = "Scriptable Objects/SkillSO")]
public class SkillSO : ScriptableObject
{
    public string skillName;
    public Sprite skillIcon;
    public int skillCost;
    public float skillDuplicator;
    public float skillRange;
    public EEntityType targetType;
    public ESkillType skillType;
}
