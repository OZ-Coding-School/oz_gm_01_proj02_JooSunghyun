using System;

[Serializable]
public class Skill_NormalAttack : ISkillAction
{
    private float mDamage;

    public Skill_NormalAttack() { }

    public void SkillAction(Entity caster, Entity target)
    {
        if (target.TryGetComponent(out IDamageable damageable)) 
        {
            mDamage = caster.GetUnitData().unitAttack;
            damageable.TakeDamage(mDamage);
        }
    }
}

[Serializable]
public class Skill_NormalHeal : ISkillAction
{
    private float mHealAmount;

    public Skill_NormalHeal() { }

    public void SkillAction(Entity caster, Entity target)
    {
        if (target.TryGetComponent(out IDamageable damageable))
        {
            mHealAmount = caster.GetUnitData().unitAttack;
            damageable.Heal(mHealAmount);
        }
    }
}
