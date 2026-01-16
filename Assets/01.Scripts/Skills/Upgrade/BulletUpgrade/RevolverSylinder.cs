using System.Collections.Generic;
using UnityEngine;

public class RevolverSylinder
{
    private EBulletType[] mBullets = new EBulletType[6];
    private Dictionary<EBulletType, int> mBulletWeights = new Dictionary<EBulletType, int>();

    public RevolverSylinder() 
    {
        foreach (EBulletType type in System.Enum.GetValues(typeof(EBulletType)))
        {
            mBulletWeights[type] = 1;
        }
    }

    public void Reload() 
    {
        for (int i = 0; i < mBullets.Length; i++) 
        {
            mBullets[i] = GetRandomBullet();
        }
    }

    public EBulletType Spin() 
    {
        int randIndex = Random.Range(0, mBullets.Length);
        return mBullets[randIndex];
    }

    public EBulletType[] GetBullets() 
    {
        return mBullets;
    }

    public void BoostBulletChance(EBulletType type, int amount) 
    {
        if (mBulletWeights.ContainsKey(type)) 
        {
            mBulletWeights[type] += amount;
        }
    }

    private EBulletType GetRandomBullet() 
    {
        int totalWeight = 0;
        foreach (var kv in mBulletWeights) 
        {
            totalWeight += kv.Value;
        }
        
        int rand = Random.Range(0, totalWeight);
        foreach (var kv in mBulletWeights) 
        {
            if (rand < kv.Value) 
            {
                return kv.Key;
            }
            rand -= kv.Value;
        }
        return EBulletType.Normal;
    }
}
