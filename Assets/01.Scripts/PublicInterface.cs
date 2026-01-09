using UnityEngine;

public interface IDamageable 
{
    //데미지 받기
    public void TakeDamage(float damage);
    public void Heal(float healAmount);

    //사망처리
    public void Death();
}

public interface IDisposable 
{
    public void Dispose();
}

public interface ISkillAction 
{
    public void SkillAction(Entity caster, Entity target);
}