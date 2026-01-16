
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
