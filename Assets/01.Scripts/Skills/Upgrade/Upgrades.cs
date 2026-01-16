
using System.Diagnostics;

public class AttackBoostEffect : IUpgradeEffect 
{
    private float mBonusDamage;
    public AttackBoostEffect(float bonusDamage) => mBonusDamage = bonusDamage;

    public void Apply(Entity caster) 
    {
        caster.currUnitAttack += mBonusDamage;
    }
}

public class DefenseBoostEffect : IUpgradeEffect
{
    private float mBonusDefense;
    public DefenseBoostEffect(float bonusDefense) => mBonusDefense = bonusDefense;

    public void Apply(Entity caster)
    {
        caster.currUnitDefense += mBonusDefense;
    }
}

public class APBoostEffect : IUpgradeEffect
{
    private float mBonusAP;
    public APBoostEffect(float bonusAP) => mBonusAP = bonusAP;

    public void Apply(Entity caster)
    {
        caster.bonusAP += (int)mBonusAP;
        Events.RaiseAPUpdate(caster.currUnitAP + caster.bonusAP);
    }
}
public class CriticalBulletEffect : IUpgradeEffect
{
    private float mChanceWeight;

    public CriticalBulletEffect(float chanceWeight) => chanceWeight = mChanceWeight;

    public void Apply(Entity caster)
    {
        UpgradeManager.Instance.revolverCylinder.BoostBulletChance(EBulletType.Critical, (int)mChanceWeight);
    }
}
public class HealBulletEffect : IUpgradeEffect 
{
    private float mChanceWeight;

    public HealBulletEffect(float chanceWeight) => chanceWeight = mChanceWeight;

    public void Apply(Entity caster) 
    {
        UpgradeManager.Instance.revolverCylinder.BoostBulletChance(EBulletType.Heal, (int)mChanceWeight);
    }
}
