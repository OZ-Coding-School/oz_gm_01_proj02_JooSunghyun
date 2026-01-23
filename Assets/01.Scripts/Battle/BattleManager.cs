using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance { get; private set; }

    private Queue<Entity> mTurnOrder = new Queue<Entity>();
    private TurnStateMachine mTurnStateMachine;

    private WaitForSeconds mWaitForSeconds = new WaitForSeconds(2.0f);

    [Header("EventChannel")]
    [SerializeField] private GameEventChannelSO mEventChannel;
    private void Awake()
    {
        Instance = this;
    }
    private void OnEnable()
    {
        mEventChannel.OnEventRaised += HandleGameEvent;
    }
    private void OnDisable()
    {
        mEventChannel.OnEventRaised -= HandleGameEvent;
    }
    private void Start()
    {
        InitializeBattle();
    }

    public void InitializeBattle() 
    {
        SetUp();
        StartNextTurn();
    }

    private void Update()
    {
        mTurnStateMachine?.Update();
    }

    public void SetUp() 
    {
        mTurnOrder.Clear();
        mTurnStateMachine = null;

        var players = StageManager.Instance.GetPlayerUnits();
        var enemies = StageManager.Instance.GetEnemyUnits();

        foreach (var player in players)
        {
            mTurnOrder.Enqueue(player);
            var apPayload = new APUpdatePayload { ap = player.currUnitAP + player.bonusAP };
            mEventChannel.RaiseEvent(EGameEventType.APUpdate, apPayload);
        }
            foreach (var enemy in enemies) { mTurnOrder.Enqueue(enemy); }
    }


    private void StartNextTurn() 
    {
        if (mTurnOrder.Count == 0) { return; }

        Entity nextEntity = mTurnOrder.Dequeue();

        if (nextEntity == null || nextEntity.currUnitHP <= 0) 
        {
            StartNextTurn();
            return;
        }

        var skillPayload = new SkillUIUpdatePayload { skills = nextEntity.GetUnitData().skills };
        mEventChannel.RaiseEvent(EGameEventType.SkillUIUpdate, skillPayload);

        var apPayload = new APUpdatePayload { ap = nextEntity.currUnitAP + nextEntity.bonusAP };
        mEventChannel.RaiseEvent(EGameEventType.APUpdate, apPayload);

        if (nextEntity.GetUnitData().unitType == EEntityType.PlayerUnit)
        {
            //플레이어 턴 스테이트머신으로 행동 관리
            mTurnStateMachine = new PlayerTurnStateMachine(nextEntity, mEventChannel);
            StageManager.Instance.UpdateVisibility(nextEntity, nextEntity.currUnitViewRange);
        }
        else if(nextEntity.GetUnitData().unitType == EEntityType.Enemy)
        {
            //에너미는 에너미걸로
            mTurnStateMachine = new EnemyTurnStateMachine(nextEntity, mEventChannel);
        }

        mTurnStateMachine.StartTurn();
        if (nextEntity != null && nextEntity.currUnitHP > 0)
        {
            mTurnOrder.Enqueue(nextEntity);
        }
    }

    public void EndCurrentTurn() 
    {
        mTurnStateMachine.Dispose();
        BroadCastTurnInfo("End Turn");
        if (IsBattleEnd())
        {
            EndBattle();
        }
        else 
        {
            StartNextTurn();
        }
    }

    private void EndBattle() 
    {
        BroadCastTurnInfo("Battle End!");

        mTurnStateMachine?.Dispose();

        if (StageManager.Instance.GetEnemyUnits().Count == 0)
        {
            //승리
            StartCoroutine(LoadNextStageCo());
            //다시 시작은 스테이지 매니저가 맵 생성 후 호출
        }
        else if (StageManager.Instance.GetPlayerUnits().Count == 0) 
        {
            //패배처리
            BattleSceneUI.instance.GameEnd();
        }
    }

    public void BroadCastTurnInfo(string info) 
    {
        var payload = new TurnInfoPayload { info = info };
        mEventChannel.RaiseEvent(EGameEventType.TurnInfoUpdate, payload);
    }
    public void BroadCastSkillUIInfo(List<SkillSO> skills) 
    {
        var payload = new SkillUIUpdatePayload { skills = skills };
        mEventChannel.RaiseEvent(EGameEventType.SkillUIUpdate, payload);
    }
    private bool IsBattleEnd() 
    {
        if (StageManager.Instance.GetPlayerUnits().Count == 0 ||
            StageManager.Instance.GetEnemyUnits().Count == 0) 
        {
            return true;
        }
        return false;
    }
    private void CheckBattleEnd(Entity entity) 
    {
        if (IsBattleEnd()) { EndBattle(); }
    }
    private void HandleGameEvent(EGameEventType type, object payload) 
    {
        if (type == EGameEventType.EntityDied && payload is EntityDiedPayload died) 
        {
            CheckBattleEnd(died.entity);
        }
    }
    private IEnumerator LoadNextStageCo() 
    {
        yield return null;
        BroadCastTurnInfo("Load Next Map");
        yield return mWaitForSeconds;
        StageManager.Instance.LoadNextStage();
    }
}
