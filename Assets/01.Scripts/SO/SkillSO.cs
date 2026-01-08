using UnityEngine;

[CreateAssetMenu(fileName = "SkillSO", menuName = "Scriptable Objects/SkillSO")]
public class SkillSO : ScriptableObject
{
    public string skillName;
    public Sprite skillIcon;
    public int skillCost;
    public float skillDamage;
    public float skillRange;
    public EEntityType targetType;

    public virtual void Evaluate(Entity caster, IDamageable target) 
    {
        target.TakeDamage(skillDamage);
    }
}
