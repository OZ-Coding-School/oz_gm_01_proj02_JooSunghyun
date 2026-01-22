using UnityEngine;
public enum EUpgradeEffectType
{
    AttackBoost,
    DefenseBoost,
    APBoost,
    CriticalBullet,
    HealBullet
}

public static class UpgradeFactory
{
    public static IUpgradeEffect CreateEffect(UpgradeSO upgrade, GameEventChannelSO channel) 
    {
        switch (upgrade.effectType) 
        {
            case EUpgradeEffectType.AttackBoost:
                return new AttackBoostEffect(upgrade.value);
            case EUpgradeEffectType.DefenseBoost:
                return new DefenseBoostEffect(upgrade.value);
            case EUpgradeEffectType.APBoost:
                return new APBoostEffect(upgrade.value, channel);
            case EUpgradeEffectType.CriticalBullet:
                return new CriticalBulletEffect(upgrade.value);
            case EUpgradeEffectType.HealBullet:
                return new HealBulletEffect(upgrade.value);
            default:
                throw new System.NotImplementedException();
        }
    }
}
