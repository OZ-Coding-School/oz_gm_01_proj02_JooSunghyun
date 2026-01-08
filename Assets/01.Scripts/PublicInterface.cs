
//데미지를 입는 엔티티
using UnityEngine;

public interface IDamageable 
{
    //데미지 받기
    public void TakeDamage(float damage);
    //사망처리
    public void Death();
}

public interface IDisposable 
{
    public void Dispose();
}