
public abstract class BulletDecorator : ISkillAction
{
    protected ISkillAction innerSkill;
    public BulletDecorator(ISkillAction innerSkill)
    {
        this.innerSkill = innerSkill;
    }
    public virtual void SkillAction(Entity caster, Entity target) 
    {
        innerSkill.SkillAction(caster, target);
    }
}
public class CriticalBulletDecorator : BulletDecorator 
{
    public CriticalBulletDecorator(ISkillAction innerSkill) : base(innerSkill) { }
    public override void SkillAction(Entity caster, Entity target)
    {
        caster.tempAttackMultiplier = 2.0f;
        innerSkill.SkillAction(caster, target);
        caster.tempAttackMultiplier = 1.0f;
    }
}
public class HealBulletDecorator : BulletDecorator 
{
    public HealBulletDecorator(ISkillAction innerSkill) : base(innerSkill) { }

    public override void SkillAction(Entity caster, Entity target)
    {
        innerSkill.SkillAction(caster, target);

        if (caster.TryGetComponent(out IDamageable damageable))
        {
            damageable.Heal(10);
        }
    }
}
