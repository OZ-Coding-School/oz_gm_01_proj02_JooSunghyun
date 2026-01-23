using UnityEngine;

public class AttackBoostEffect : IUpgradeEffect 
{
    private float mBonusDamage;
    public AttackBoostEffect(float bonusDamage) => mBonusDamage = bonusDamage;

    public void Apply(Entity caster) 
    {
        caster.currUnitAttack = caster.baseAttack + mBonusDamage;
    }
}

public class DefenseBoostEffect : IUpgradeEffect
{
    private float mBonusDefense;
    public DefenseBoostEffect(float bonusDefense) => mBonusDefense = bonusDefense;

    public void Apply(Entity caster)
    {
        caster.currUnitDefense = caster.baseDefense + mBonusDefense;
    }
}

public class APBoostEffect : IUpgradeEffect
{
    private float mBonusAP;
    private GameEventChannelSO mEventChannel;
    public APBoostEffect(float bonusAP, GameEventChannelSO channel) 
    {
        mBonusAP = bonusAP;
        mEventChannel = channel;
    }

    public void Apply(Entity caster)
    {
        caster.bonusAP += (int)mBonusAP;

        var payload = new APUpdatePayload { ap = caster.currUnitAP + caster.bonusAP };
        mEventChannel.RaiseEvent(EGameEventType.APUpdate, payload);
    }
}
public class CriticalBulletEffect : IUpgradeEffect
{
    private float mChanceWeight;
    private bool mIsActivated = false;
    public CriticalBulletEffect(float chanceWeight) => mChanceWeight = chanceWeight;

    public void Apply(Entity caster)
    {
        if (!mIsActivated)
        {
            mIsActivated = true;
            UpgradeManager.Instance.revolverCylinder.BoostBulletChance(EBulletType.Critical, Mathf.CeilToInt(mChanceWeight));
        }
    }
}
public class HealBulletEffect : IUpgradeEffect 
{
    private float mChanceWeight;
    private bool mIsActivated = false;
    public HealBulletEffect(float chanceWeight) => mChanceWeight = chanceWeight;

    public void Apply(Entity caster)
    {
        if (!mIsActivated)
        {
            mIsActivated = true;
            UpgradeManager.Instance.revolverCylinder.BoostBulletChance(EBulletType.Heal, Mathf.CeilToInt(mChanceWeight));
        }
    }
}
