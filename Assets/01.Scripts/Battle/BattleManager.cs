using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance { get; private set; }

    private Queue<Entity> turnOrder = new Queue<Entity>();
    private TurnStateMachine turnStateMachine;

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
        turnStateMachine.Update();
    }

    public void SetUp() 
    {
        var players = StageManager.Instance.GetPlayerUnits();
        var enemies = StageManager.Instance.GetEnemyUnits();

        foreach (var player in players) { turnOrder.Enqueue(player); }
        foreach (var enemy in enemies) { turnOrder.Enqueue(enemy); }
    }


    private void StartNextTurn() 
    {
        if (turnOrder.Count == 0) { return; }

        Entity nextEntity = turnOrder.Dequeue();

        if (nextEntity.GetUnitData().unitType == EEntityType.PlayerUnit)
        {
            //플레이어 턴 스테이트머신으로 행동 관리
            turnStateMachine = new PlayerTurnStateMachine(nextEntity);
        }
        else if(nextEntity.GetUnitData().unitType == EEntityType.Enemy)
        {
            //에너미는 에너미걸로
            turnStateMachine = new EnemyTurnStateMachine(nextEntity);
        }

        turnStateMachine.StartTurn();

        turnOrder.Enqueue(nextEntity);
    }

    public void EndCurrentTurn() 
    {
        StartNextTurn();
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
