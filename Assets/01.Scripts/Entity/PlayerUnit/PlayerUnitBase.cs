using UnityEngine;

public class PlayerUnitBase : Entity, IDamageable, ICameraChaseable
{
    public void TakeDamage(float damage, Entity caster) 
    {
        int realDamage = (int)(damage - currUnitDefense / 100);
 
        currUnitHP = Mathf.Clamp(currUnitHP - realDamage, 0, GetUnitData().unitHP);
        mHealthBar.SetHealth(currUnitHP);

        EffectManager.Instance.PlayEffect(EEffectType.HitSpark, transform.position);

        var payload = new DamagePayload { damage = realDamage, attacker = caster, target = this };
        mEventChannel.RaiseEvent(EGameEventType.DamageDealt, payload);

        if (currUnitHP <= 0) { Death(); }
    }
    public void Heal(float healAmount)
    {
        currUnitHP = Mathf.Clamp(currUnitHP + Mathf.Abs(healAmount*2), 0, GetUnitData().unitHP);
        mHealthBar.SetHealth(currUnitHP);
    }
    public void Death()
    {
        var payload = new EntityDiedPayload { entity = this };
        mEventChannel.RaiseEvent(EGameEventType.EntityDied, payload);
    }
}
