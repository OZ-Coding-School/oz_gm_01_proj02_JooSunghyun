using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTurnStateMachine : TurnStateMachine
{
    private Entity mCurrEntity;
    private WaitForSeconds mWaitForSeconds = new WaitForSeconds(UnityEngine.Random.Range(0.6f, 1.2f));

    private GameEventChannelSO mEventChannel;
    public EnemyTurnStateMachine(Entity entity, GameEventChannelSO channel) : base(entity)
    {
        mCurrEntity = entity;
        mEventChannel = channel;
    }

    public override void StartTurn()
    {
        mCurrEntity.currUnitAP += mCurrEntity.GetUnitData().unitAP;
        //이벤트 발행
        BattleManager.Instance.BroadCastTurnInfo("Enemy Turn Start");
        BattleManager.Instance.BroadCastSkillUIInfo(mCurrEntity.GetUnitData().skills);

        var apPayload = new APUpdatePayload { ap = mCurrentEntity.currUnitAP + mCurrentEntity.bonusAP };
        mEventChannel.RaiseEvent(EGameEventType.APUpdate, apPayload);

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

        SkillSO attackSkill = mCurrEntity.GetUnitData().skills[0];
        int skillRange = (int)attackSkill.skillRange;

        int dist = AStarPathFinder.Heuristic(currPos, target.GetPosition());

        if (dist <= skillRange)
        {
            BattleManager.Instance.BroadCastTurnInfo("Enemy Attack");
            yield return mWaitForSeconds;

            ISkillAction baseSkill = SkillFactory.CreateSkill(attackSkill);
            List<EBulletType> enemyBullets = new List<EBulletType> { GetRandomBullet() };
            ISkillAction decoratedSkill = BulletFactory.ApplyBulletEffect(enemyBullets, baseSkill);

            mActionQueue.Enqueue(new AttackNode(decoratedSkill, attackSkill, target, mEventChannel));
        }
        else 
        {
            var reachableTiles = AStarPathFinder.GetReachableTiles(
            currPos, mCurrEntity.currUnitAP, StageManager.Instance.GetWalkableTiles());

            Vector3Int targetPos = FindClosestTile(reachableTiles, target.GetPosition(), skillRange);
          
            BattleManager.Instance.BroadCastTurnInfo("Moving...");
            yield return mWaitForSeconds;
            mActionQueue.Enqueue(new MoveNode(targetPos, mEventChannel));
        }
    }

    private IEnumerator WaitCo() 
    {
        yield return mWaitForSeconds;
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
                    BattleManager.Instance.StartCoroutine(WaitCo());

                    Entity target = StageManager.Instance.GetPlayerUnits()[0];
                    SkillSO attackSkill = mCurrEntity.GetUnitData().skills[0];

                    ISkillAction baseSkill = SkillFactory.CreateSkill(attackSkill);

                    List<EBulletType> enemyBullets = new List<EBulletType>();
                    enemyBullets.Add(GetRandomBullet());

                    ISkillAction decoratedSkill = BulletFactory.ApplyBulletEffect(enemyBullets, baseSkill);

                    mActionQueue.Enqueue(new AttackNode(decoratedSkill, attackSkill, target, mEventChannel));
                }
                else if (currentNode is AttackNode) 
                {
                    mActionQueue.Enqueue(new EndTurnNode());
                }
            }
        }
    }

    private Vector3Int FindClosestTile(HashSet<Vector3Int> reachableTiles, Vector3Int playerPos, int range) 
    {
        Vector3Int bestTile = mCurrEntity.GetPosition();
        int maxDist = -1;

        foreach (var tile in reachableTiles) 
        {
            if (tile == playerPos) continue;
            int dist = AStarPathFinder.Heuristic(playerPos, tile);
            if (dist <= range && dist > maxDist) 
            {
                maxDist = dist;
                bestTile = tile;
            }
        }
        if (maxDist == -1) 
        {
            int minDist = int.MaxValue;
            foreach (var tile in reachableTiles) 
            {
                int dist = AStarPathFinder.Heuristic(playerPos, tile);
                if (dist < minDist) 
                {
                    minDist = dist;
                    bestTile = tile;
                }
            }
        }
        return bestTile;
    }

    private EBulletType GetRandomBullet() 
    {
        int i = UnityEngine.Random.Range(0, Enum.GetValues(typeof(EBulletType)).Length);
        EBulletType[] bullets = (EBulletType[])Enum.GetValues(typeof(EBulletType));
        return bullets[i];
    }
}
