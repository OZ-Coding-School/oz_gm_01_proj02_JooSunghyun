using UnityEngine;

public class EnemyBase : Entity, IDamageable, ICameraChaseable
{
    public void TakeDamage(float damage, Entity caster)
    {
        int realDamage = (int)(damage - currUnitDefense / 100);
        //일단 임시. 나중에 데미지 공식 고정적으로 만든거 쓰기
        currUnitHP = Mathf.Clamp(currUnitHP - realDamage, 0, GetUnitData().unitHP);
        mHealthBar.SetHealth(currUnitHP);

        EffectManager.Instance.PlayEffect(EEffectType.HitSpark, transform.position);

        var payload = new DamagePayload { damage = realDamage, attacker = caster, target = this };
        mEventChannel.RaiseEvent(EGameEventType.DamageDealt, payload);

        if (currUnitHP <= 0) { Death(); }
    }
    public void Heal(float healAmount)
    {
        currUnitHP = Mathf.Clamp(currUnitHP + healAmount, 0, GetUnitData().unitHP);
        mHealthBar.SetHealth(currUnitHP);
    }
    public void Death()
    {
        var payload = new EntityDiedPayload { entity = this };
        mEventChannel.RaiseEvent(EGameEventType.EntityDied, payload);
    }
}
