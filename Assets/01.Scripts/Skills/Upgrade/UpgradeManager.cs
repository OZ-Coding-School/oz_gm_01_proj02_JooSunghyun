using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance { get; private set; }
    //레벨업 강화 시스템
    //총알 추첨 시스템
    public RevolverSylinder revolverCylinder;

    public int playerLevel = 0;
    public float playerExp = 0;

    private int[] mExpTable
        = { 5, 10, 15, 20, 30, 40, 50, 60, 75, 90, 105, 120, 140, 160, 180, 220, 280 };

    public List<UpgradeSO> upgradePool;
    public List<IUpgradeEffect> activeUpgrades = new List<IUpgradeEffect>();

    private Entity mPlayer;
    private void Awake()
    {
        Instance = this;
        revolverCylinder = new RevolverSylinder();
    }

    private void OnEnable()
    {
        Events.OnStageChange += UpdateApplyUpgrade;
        Events.OnUpgradeSelected += ApplyUpgrade;
        Events.OnEntityDied += AddExp;
    }
    private void OnDisable()
    {
        Events.OnStageChange -= UpdateApplyUpgrade;
        Events.OnUpgradeSelected -= ApplyUpgrade;
        Events.OnEntityDied -= AddExp;
    }

    public void AddExp(Entity entity)
    {
        if (entity.GetUnitData().unitType == EEntityType.Enemy) 
        {
            float amount = entity.GetUnitData().unitExp;
            playerExp += amount;
        }
        CheckLevelUp();
    }

    private void CheckLevelUp()
    {
        if (playerLevel < mExpTable.Length && playerExp >= mExpTable[playerLevel])
        {
            playerLevel++;
            playerExp = 0;

            List<UpgradeSO> choices = GetRandomUpgrades(3);

            Events.RaiseLevelUp(playerLevel, choices);
        }
    }

    private List<UpgradeSO> GetRandomUpgrades(int count)
    {
        List<UpgradeSO> result = new List<UpgradeSO>();
        List<UpgradeSO> poolCopy = new List<UpgradeSO>(upgradePool); //원본 복사
        for (int i = 0; i < count; i++)
        {
            int rand = Random.Range(0, poolCopy.Count);
            result.Add(poolCopy[rand]);
            poolCopy.RemoveAt(rand);
        }
        return result;
    }

    private void ApplyUpgrade(UpgradeSO upgrade)
    {
        if (GameObject.FindWithTag("Player").TryGetComponent(out Entity entity))
        {
            mPlayer = entity;
        }

        IUpgradeEffect effect = UpgradeFactory.CreateEffect(upgrade);
        activeUpgrades.Add(effect);
        effect.Apply(mPlayer);
    }

    private void UpdateApplyUpgrade(int i) 
    {
        if (activeUpgrades.Count > 0) 
        {
            foreach (var upgrade in activeUpgrades) 
            {
                upgrade.Apply(mPlayer);
            }
        }
    }
}
