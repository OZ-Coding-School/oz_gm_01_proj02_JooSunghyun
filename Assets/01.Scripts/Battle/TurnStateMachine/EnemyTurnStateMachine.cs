using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTurnStateMachine : TurnStateMachine
{
    private Entity mCurrEntity;
    private WaitForSeconds mWaitForSeconds = new WaitForSeconds(Random.Range(0.6f, 1.2f));

    public EnemyTurnStateMachine(Entity entity) : base(entity)
    {
        mCurrEntity = entity;
    }

    public override void StartTurn()
    {
        mCurrEntity.currUnitAP += mCurrEntity.GetUnitData().unitAP;
        //이벤트 발행
        BattleManager.Instance.BroadCastTurnInfo("Enemy Turn Start");
        BattleManager.Instance.BroadCastSkillUIInfo(mCurrEntity.GetUnitData().skills);
        Events.RaiseAPUpdate(mCurrEntity.currUnitAP);
        BattleManager.Instance.StartCoroutine(EnemyActionCo());
    }

    private IEnumerator EnemyActionCo() 
    {
        BattleManager.Instance.BroadCastTurnInfo("Enemy turn...");
        yield return mWaitForSeconds;
        //타겟 선택
        var players = StageManager.Instance.GetPlayerUnits();
        Entity target = players[0];
        
        //이동
        Vector3Int currPos = mCurrEntity.GetPosition() + new Vector3Int(0, -1, 0);
        var reachableTiles = AStarPathFinder.GetReachableTiles(
            currPos, mCurrEntity.currUnitAP, StageManager.Instance.GetWalkableTiles());

        Vector3Int targetPos = target.GetPosition();
        if (!reachableTiles.Contains(targetPos)) 
        {
            targetPos = FindClosestTile(reachableTiles, target.GetPosition());
        }
        BattleManager.Instance.BroadCastTurnInfo("Moving...");
        yield return mWaitForSeconds;
        mActionQueue.Enqueue(new MoveNode(targetPos));
    }

    public override void Update() 
    {
        if (mActionQueue.Count > 0) 
        {
            BTNode currentNode = mActionQueue.Peek();
            if (currentNode.Evaluate(mCurrEntity)) 
            {
                mActionQueue.Dequeue();

                if (currentNode is MoveNode)
                {
                    Entity target = StageManager.Instance.GetPlayerUnits()[0];
                    SkillSO attackSkill = mCurrEntity.GetUnitData().skills[0];
                    mActionQueue.Enqueue(new AttackNode(attackSkill, target));
                }
                else if (currentNode is AttackNode) 
                {
                    mActionQueue.Enqueue(new EndTurnNode());
                }
            }
        }
    }

    private void EndTurn() 
    {
        BattleManager.Instance.EndCurrentTurn();
    }

    private Vector3Int FindClosestTile(HashSet<Vector3Int> reachableTiles, Vector3Int playerPos) 
    {
        Vector3Int closest = playerPos;
        int minDist = int.MaxValue;

        foreach (var tile in reachableTiles) 
        {
            int dist = Mathf.Abs(playerPos.x - tile.x) + Mathf.Abs(playerPos.z - tile.z);
            if (dist < minDist) 
            {
                minDist = dist;
                closest = tile;
            }
        }
        return closest;
    }
}
