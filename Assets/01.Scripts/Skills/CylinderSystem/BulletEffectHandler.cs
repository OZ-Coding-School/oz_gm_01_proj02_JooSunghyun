using UnityEngine;

public class BulletEffectHandler
{
    public void ApplyEffect(EBulletType bullet, Entity caster, Entity target) 
    {
        switch (bullet) 
        {
            case EBulletType.Critical:
                caster.tempAttackMultiplier = 2.0f; 
                break;
            case EBulletType.Heal:
                if (caster.TryGetComponent(out IDamageable damageable)) 
                {
                    damageable.Heal(10);
                }
                break;
            case EBulletType.Normal:
            default:
                BattleManager.Instance.BroadCastTurnInfo("Bullet Effect: Normal");
            break;
        }
    }
}
