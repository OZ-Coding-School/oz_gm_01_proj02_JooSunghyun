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

    [Header("Event Channel")]
    [SerializeField] private GameEventChannelSO mEventChannel;
    private void Awake()
    {
        Instance = this;
        revolverCylinder = new RevolverSylinder();
    }

    private void OnEnable()
    {
        mEventChannel.OnEventRaised += HandleGameEvent;
    }
    private void OnDisable()
    {
        mEventChannel.OnEventRaised -= HandleGameEvent;
    }

    private void HandleGameEvent(EGameEventType type, object payload)
    {
        switch (type)
        {
            case EGameEventType.StageChange:
                if (payload is StageChangePayload stagePayload)
                    UpdateApplyUpgrade(stagePayload.stageLevel);
                break;

            case EGameEventType.UpgradeSelected:
                if (payload is UpgradeSelectedPayload upgradePayload)
                    ApplyUpgrade(upgradePayload.selected);
                break;

            case EGameEventType.EntityDied:
                if (payload is EntityDiedPayload diedPayload)
                    AddExp(diedPayload.entity);
                break;

            case EGameEventType.PlayerSpawned:
                if (payload is PlayerSpawnedPayload p)
                    mPlayer = p.entity;
                break;
        }
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

            var payload = new LevelUpPayload { newLevel = playerLevel, choices = choices };
            mEventChannel.RaiseEvent(EGameEventType.LevelUp, payload);
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
        CheckPlayer();

        IUpgradeEffect effect = UpgradeFactory.CreateEffect(upgrade);
        activeUpgrades.Add(effect);
        effect.Apply(mPlayer);
    }

    private void UpdateApplyUpgrade(int i) 
    {
        CheckPlayer();

        if (activeUpgrades.Count > 0) 
        {
            foreach (var upgrade in activeUpgrades) 
            {
                upgrade.Apply(mPlayer);
            }
        }
    }

    private void CheckPlayer() 
    {
        if (!mPlayer.isActiveAndEnabled)
        {
            if (GameObject.FindWithTag(Define.Player).TryGetComponent(out Entity entity))
            {
                mPlayer = entity;
            }
        }
    }
}
