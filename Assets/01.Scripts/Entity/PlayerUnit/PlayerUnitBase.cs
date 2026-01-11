using Unity;
using UnityEngine;

public class PlayerUnitBase : Entity, IDamageable, ICameraChaseable
{
    public void TakeDamage(float damage) 
    {
        //일단 임시. 나중에 데미지 공식 고정적으로 만든거 쓰기
        mUnitHP = Mathf.Clamp(mUnitHP - damage, 0, GetUnitData().unitHP);
        mHealthBar.SetHealth(mUnitHP);
    }
    public void Heal(float healAmount)
    {
        mUnitHP = Mathf.Clamp(mUnitHP + Mathf.Abs(healAmount*2), 0, GetUnitData().unitHP);
        mHealthBar.SetHealth(mUnitHP);
    }
    public void Death()
    {

    }
}
