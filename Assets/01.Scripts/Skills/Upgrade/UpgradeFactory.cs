using UnityEngine;

public static class UpgradeFactory
{
    public static IUpgradeEffect CreateEffect(UpgradeSO upgrade) 
    {
        switch (upgrade.effectType) 
        {
            case EUpgradeEffectType.AttackBoost:
                return new AttackBoostEffect(upgrade.value);
            case EUpgradeEffectType.DefenseBoost:
                return new DefenseBoostEffect(upgrade.value);
            case EUpgradeEffectType.APBoost:
                return new APBoostEffect(upgrade.value);
            default:
                throw new System.NotImplementedException();
        }
    }
}
