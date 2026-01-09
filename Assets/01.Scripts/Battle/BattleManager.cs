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
        mTurnStateMachine.Update();
    }

    public void SetUp() 
    {
        var players = StageManager.Instance.GetPlayerUnits();
        var enemies = StageManager.Instance.GetEnemyUnits();

        foreach (var player in players) { mTurnOrder.Enqueue(player); }
        foreach (var enemy in enemies) { mTurnOrder.Enqueue(enemy); }
    }


    private void StartNextTurn() 
    {
        if (mTurnOrder.Count == 0) { return; }

        Entity nextEntity = mTurnOrder.Dequeue();

        if (nextEntity.GetUnitData().unitType == EEntityType.PlayerUnit)
        {
            //플레이어 턴 스테이트머신으로 행동 관리
            mTurnStateMachine = new PlayerTurnStateMachine(nextEntity);
        }
        else if(nextEntity.GetUnitData().unitType == EEntityType.Enemy)
        {
            //에너미는 에너미걸로
            mTurnStateMachine = new EnemyTurnStateMachine(nextEntity);
        }

        mTurnStateMachine.StartTurn();

        mTurnOrder.Enqueue(nextEntity);
    }

    public void EndCurrentTurn() 
    {
        StartNextTurn();
    }

    public void BroadCastTurnInfo(string info) 
    {
        BattleEvents.RaiseTurnInfoUpdate(info);
    }
    public void BroadCastSkillUIInfo(List<SkillSO> skills) 
    {
        BattleEvents.RaiseSkillUIUpdate(skills);
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


    //승리조건 관리
    //전투가 끝나면 게임 매니저에 전달
    //스테이지 매니저에 스테이지 청소 지시(필요하면)
}
