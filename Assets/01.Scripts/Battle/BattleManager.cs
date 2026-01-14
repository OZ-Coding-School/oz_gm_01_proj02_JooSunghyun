using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance { get; private set; }

    private Queue<Entity> mTurnOrder = new Queue<Entity>();
    private TurnStateMachine mTurnStateMachine;

    private void Awake()
    {
        Instance = this;
    }
    private void OnEnable()
    {
        Events.OnEntityDied += CheckBattleEnd;
    }
    private void OnDisable()
    {
        Events.OnEntityDied -= CheckBattleEnd;
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

        var players = StageManager.Instance.GetPlayerUnits();
        var enemies = StageManager.Instance.GetEnemyUnits();

        Debug.Log($"플레이어 : {players.Count}, 에너미 : {enemies.Count}");

        foreach (var player in players) { mTurnOrder.Enqueue(player); }
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

        Events.RaiseSkillUIUpdate(nextEntity.GetUnitData().skills);
        Events.RaiseAPUpdate(nextEntity.currUnitAP);

        if (nextEntity.GetUnitData().unitType == EEntityType.PlayerUnit)
        {
            //플레이어 턴 스테이트머신으로 행동 관리
            mTurnStateMachine = new PlayerTurnStateMachine(nextEntity);
            StageManager.Instance.UpdateVisibility(nextEntity, nextEntity.currUnitViewRange);
        }
        else if(nextEntity.GetUnitData().unitType == EEntityType.Enemy)
        {
            //에너미는 에너미걸로
            mTurnStateMachine = new EnemyTurnStateMachine(nextEntity);
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
            StageManager.Instance.LoadNextStage();
            //다시 시작은 스테이지 매니저가 맵 생성 후 호출
        }
        else if (StageManager.Instance.GetPlayerUnits().Count == 0) 
        {
            //패배처리
        }
    }

    public void BroadCastTurnInfo(string info) 
    {
        Events.RaiseTurnInfoUpdate(info);
    }
    public void BroadCastSkillUIInfo(List<SkillSO> skills) 
    {
        Events.RaiseSkillUIUpdate(skills);
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

    //승리조건 관리
    //전투가 끝나면 게임 매니저에 전달
    //스테이지 매니저에 스테이지 청소 지시(필요하면)
}
