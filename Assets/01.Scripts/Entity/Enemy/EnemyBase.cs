using UnityEngine;

public class EnemyBase : Entity, IDamageable, ICameraChaseable
{
    public void TakeDamage(float damage, Entity caster)
    {
        //일단 임시. 나중에 데미지 공식 고정적으로 만든거 쓰기
        currUnitHP = Mathf.Clamp(currUnitHP - damage, 0, GetUnitData().unitHP);
        mHealthBar.SetHealth(currUnitHP);
        Events.RaiseDamageDealt(damage, caster, this);
        if (currUnitHP <= 0) { Death(); }
    }
    public void Heal(float healAmount)
    {
        currUnitHP = Mathf.Clamp(currUnitHP + healAmount, 0, GetUnitData().unitHP);
        mHealthBar.SetHealth(currUnitHP);
    }
    public void Death()
    {
        Events.RaiseEntityDied(this);
    }
}
