using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTurnStateMachine : TurnStateMachine
{
    private Entity mPlayerEntity;
    private SkillSO mSelectedSkill;
    private Entity mSelectedTarget;
    private List<EBulletType> mSelectedBullets;
    private RevolverSylinder mCylinder;
    private ISkillAction mPreparedSkill;

    private GameEventChannelSO mEventChannel;

    public PlayerTurnStateMachine(Entity entity, GameEventChannelSO channel) : base(entity)
    {
        mPlayerEntity = entity;
        mCylinder = UpgradeManager.Instance.revolverCylinder;
        mEventChannel = channel;

        mEventChannel.OnEventRaised += HandleGameEvent;
    }

    public override void StartTurn()
    {
        mActionQueue.Clear();
        mPlayerEntity.currUnitAP += mPlayerEntity.GetUnitData().unitAP;
     
        BattleManager.Instance.BroadCastTurnInfo("Player Turn");
        BattleManager.Instance.BroadCastSkillUIInfo(mPlayerEntity.GetUnitData().skills);

        var apPayload = new APUpdatePayload { ap = mPlayerEntity.currUnitAP + mPlayerEntity.bonusAP };
        mEventChannel.RaiseEvent(EGameEventType.APUpdate, apPayload);
    }

    public override void Update()
    {
        if (mActionQueue.Count > 0)
        {
            BTNode currentNode = mActionQueue.Peek();

            if (currentNode.Evaluate(mPlayerEntity))
            {
                mActionQueue.Dequeue();
                if (currentNode is WaitInputNode waitInputNode)
                {
                    TileBase selectedTile = waitInputNode.GetSelectedTile();
                    if (selectedTile != null)
                    {
                        mActionQueue.Enqueue(new MoveNode(selectedTile.GetPosition(), mEventChannel));
                    }
                }
                else if (currentNode is MoveNode)
                {
                    AfterAction();
                }
                else if (currentNode is TargetSelectNode targetNode)
                {
                    mSelectedTarget = targetNode.GetSelectedTarget();
                    if (mSelectedSkill != null && mSelectedTarget != null)
                    {
                        ISkillAction baseSkill = SkillFactory.CreateSkill(mSelectedSkill);
                        ISkillAction decoratedSkill = baseSkill;

                        if (mSelectedSkill.targetType == EEntityType.Enemy && mSelectedBullets != null) 
                        {
                            decoratedSkill = BulletFactory.ApplyBulletEffect(mSelectedBullets, baseSkill);
                        }

                        mPreparedSkill = decoratedSkill;
                        mActionQueue.Enqueue(new AttackNode(decoratedSkill, mSelectedSkill, mSelectedTarget, mEventChannel));
                    }
                    else
                    {
                        mActionQueue.Enqueue(new WaitNode());
                        BattleManager.Instance.BroadCastTurnInfo("No valid Target");
                    }
                }
                else if (currentNode is AttackNode) 
                {
                    AfterAction();
                }
            }
        }
    }
    private void HandleGameEvent(EGameEventType type, object payload) 
    {
        switch (type)
        {
            case EGameEventType.MoveSelected:
                HandleMoveSelected();
                break;

            case EGameEventType.SkillSelected:
                if (payload is SkillSelectedPayload skillPayload)
                    HandleSkillSelected(skillPayload.skillIndex);
                break;

            case EGameEventType.CylinderSpinEnd:
                if (payload is CylinderSpinPayload spinPayload)
                    HandleCylinderSpin(spinPayload.bullets);
                break;

            case EGameEventType.TurnSkip:
                HandleTurnSkip();
                break;
        }
    }
    private void HandleMoveSelected()
    {
        mActionQueue.Enqueue(new WaitInputNode(mPlayerEntity));
        BattleManager.Instance.BroadCastTurnInfo("Select tile to move");
    }
    private void HandleCylinderSpin(List<EBulletType> bullets) 
    {
        mSelectedBullets = bullets;
        mActionQueue.Enqueue(new TargetSelectNode(mSelectedSkill, mEventChannel));
        BattleManager.Instance.BroadCastTurnInfo("Select target to use");
    }
    private void HandleSkillSelected(int skillIndex)
    {
        var skills = mPlayerEntity.GetUnitData().skills;
        if (skills.Count > skillIndex) 
        {
            SkillSO selectedSkill = skills[skillIndex];
            if (mPlayerEntity.currUnitAP >= selectedSkill.skillCost)
            {
                mSelectedSkill = selectedSkill;
                if (mSelectedSkill.targetType == EEntityType.Enemy)
                {
                    var enemies = StageManager.Instance.GetEnemyUnits();
                    bool hasVisibleEnemy = false;
                    Vector3Int casterPos = mPlayerEntity.GetPosition() + new Vector3Int(0, -1, 0);

                    foreach (var enemy in enemies) 
                    {
                        Vector3Int enemyPos = enemy.GetPosition() + new Vector3Int(0, -1, 0);
                        int dist = AStarPathFinder.Heuristic(casterPos, enemyPos);
                        if (dist <= mSelectedSkill.skillRange && dist <= mPlayerEntity.currUnitViewRange) 
                        {
                            hasVisibleEnemy = true;
                            break;
                        }
                    }

                    if (hasVisibleEnemy) 
                    {
                        //적 대상
                        mEventChannel.RaiseEvent(EGameEventType.OpenCylinderUI);
                        mActionQueue.Enqueue(new WaitNode());
                    }
                    else
                    {
                        BattleManager.Instance.BroadCastTurnInfo("No Valid Target");
                        mActionQueue.Enqueue(new WaitNode());
                    }
                }
                else 
                {
                    //아군 대상
                    mActionQueue.Enqueue(new TargetSelectNode(mSelectedSkill, mEventChannel));
                    BattleManager.Instance.BroadCastTurnInfo("Select target to use");
                }
            }
            else 
            {
                BattleManager.Instance.BroadCastTurnInfo("Not enough AP");
            }
        }
    }
    private void HandleTurnSkip() 
    {
        mActionQueue.Enqueue(new EndTurnNode());
        BattleManager.Instance.BroadCastTurnInfo("Turn end");
    }
    private void AfterAction() 
    {
        if (mPlayerEntity.currUnitAP <= 0)
        {
            mActionQueue.Enqueue(new EndTurnNode());
        }
        else 
        {
            mActionQueue.Enqueue(new WaitNode());
            BattleManager.Instance.BroadCastTurnInfo("Choose your action");
            BattleManager.Instance.BroadCastSkillUIInfo(mPlayerEntity.GetUnitData().skills);
        }
    }

    public override void Dispose()
    {
        mEventChannel.OnEventRaised -= HandleGameEvent;
    }
}
