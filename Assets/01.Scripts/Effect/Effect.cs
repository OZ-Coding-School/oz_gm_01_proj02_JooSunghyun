using UnityEngine;
using System;

public class Effect : MonoBehaviour
{
    private Action mOnComplete;

    [SerializeField] private ParticleSystem mParticle;

    public void Play(Action onComplete = null)
    {
        mOnComplete = onComplete;

        gameObject.SetActive(true);

        if (mParticle != null)
        {
            mParticle.Play();
            // 파티클 길이에 맞춰 자동 반환
            Invoke(nameof(ReturnToPool), mParticle.main.duration);
        }
        else
        {
            Invoke(nameof(ReturnToPool), 0.5f);
        }
    }

    private void ReturnToPool()
    {
        mOnComplete?.Invoke();
        gameObject.SetActive(false);
    }

}
