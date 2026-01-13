using UnityEngine;

public class RevolverSylinder
{
    private EBulletType[] mBullets = new EBulletType[6];

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

    private EBulletType GetRandomBullet() 
    {
        int bulletTypeCount = System.Enum.GetValues(typeof(EBulletType)).Length;
        int rand = Random.Range(0, bulletTypeCount);
        return (EBulletType)rand;
    }
}
