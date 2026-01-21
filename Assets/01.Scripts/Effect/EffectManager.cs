using UnityEngine;

public enum EEffectType { MuzzleFlash, BulletTrail, HitSpark }

public class EffectManager : MonoBehaviour
{
    public static EffectManager Instance;

    [SerializeField] private Effect mMuzzleFlashPrefab;
    [SerializeField] private Effect mBulletTrailPrefab;
    [SerializeField] private Effect mHitSparkPrefab;

    private void Awake()
    {
        Instance = this;

        PoolManager.Instance.CreatePool(mMuzzleFlashPrefab, 5, null);
        PoolManager.Instance.CreatePool(mBulletTrailPrefab, 5, null);
        PoolManager.Instance.CreatePool(mHitSparkPrefab, 5, null);
    }

    public void PlayEffect(EEffectType type, Vector3 position) 
    {
        Effect prefab = null;
        switch (type) 
        {
            case EEffectType.MuzzleFlash:
                prefab = mMuzzleFlashPrefab;
                break;
            case EEffectType.HitSpark:
                prefab = mHitSparkPrefab;
                break;
        }
        if (prefab != null) 
        {
            Effect effect = PoolManager.Instance.GetFromPool(prefab);
            if (effect != null)
            {
                effect.transform.position = position;
                effect.gameObject.SetActive(true);

                effect.Play(() =>{ PoolManager.Instance.ReturnPool(effect); });
            }

        }
    }

    public void PlayBulletTrail(Vector3 startPos, Vector3 targetPos) 
    {
        if (mBulletTrailPrefab != null)
        {
            Effect trailEffect = PoolManager.Instance.GetFromPool(mBulletTrailPrefab);
            if (trailEffect != null)
            {
                trailEffect.transform.position = startPos;
                trailEffect.gameObject.SetActive(true);

                TrailEffect trail = trailEffect.GetComponent<TrailEffect>();
                if (trail != null)
                {
                    trail.Init(startPos, targetPos, () => {  PoolManager.Instance.ReturnPool(trailEffect); });
                }
            }
        }
    }
}
