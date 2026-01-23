using UnityEngine;

public class BattleStats : MonoBehaviour
{
    public static BattleStats Instance { get; private set; }

    public int turnCount { get; private set; }
    public int killCount { get; private set; }
    public int skillUsedCount { get; private set; }
    public float totalDamageDealt { get; private set; }
    public float totalDamageTaken { get; private set; }

    [Header("Event Channel")]
    [SerializeField] private GameEventChannelSO mEventChannel;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        mEventChannel.OnEventRaised += HandleGameEvent;
        ResetStats();
    }
    private void OnDisable()
    {
        mEventChannel.OnEventRaised -= HandleGameEvent;
    }

    private void HandleGameEvent(EGameEventType type, object payload) 
    {
        switch (type) 
        {
            case EGameEventType.EntityDied:
                if (payload is EntityDiedPayload died) 
                {
                    if (died.entity.GetUnitData().unitType == EEntityType.Enemy) killCount++;
                }
                break;

            case EGameEventType.DamageDealt:
                if (payload is DamagePayload dmg) 
                {
                    totalDamageDealt += dmg.damage;
                    if (dmg.target.GetUnitData().unitType == EEntityType.PlayerUnit)
                        totalDamageTaken += dmg.damage;
                }
                break;

            case EGameEventType.SkillUsed:
                if (payload is SkillUsedPayload )
                {
                    skillUsedCount++;
                }
                break;

            case EGameEventType.TurnInfoUpdate:
                turnCount++;
                break;
        }
    }

    public void ResetStats() 
    {
        turnCount = 0;
        killCount = 0;
        skillUsedCount = 0;
        totalDamageDealt = 0;
        totalDamageTaken = 0;
    }

    public string GetRank() 
    {
        if (killCount >= 40 && totalDamageTaken < 100 && turnCount <= 50)
            return "S";
        else if (killCount >= 20 && totalDamageTaken < 150 && turnCount <= 80)
            return "A";
        else if (killCount >= 10)
            return "B";
        else
            return "C";
    }
}
