using NUnit.Framework;
using System.Collections.Generic;

public class PlayerTurnStateMachine : TurnStateMachine
{
    private Entity mPlayerEntity;
    private SkillSO mSelectedSkill;
    private Entity mSelectedTarget;
    private List<EBulletType> mSelectedBullets;
    private RevolverSylinder mCylinder;
    private ISkillAction mPreparedSkill;

    public PlayerTurnStateMachine(Entity entity) : base(entity)
    {
        mPlayerEntity = entity;
        mCylinder = UpgradeManager.Instance.revolverCylinder;

        Events.OnMoveSelected += HandleMoveSelected;
        Events.OnSkillSelected += HandleSkillSelected;
        Events.OnCylinderSpin += HandleCylinderSpin;
        Events.OnTurnSkip += HandleTurnSkip;
    }

    public override void StartTurn()
    {
        mActionQueue.Clear();
        mPlayerEntity.currUnitAP += mPlayerEntity.GetUnitData().unitAP;
     
        BattleManager.Instance.BroadCastTurnInfo("Player Turn");
        BattleManager.Instance.BroadCastSkillUIInfo(mPlayerEntity.GetUnitData().skills);
        Events.RaiseAPUpdate(mPlayerEntity.currUnitAP + mPlayerEntity.bonusAP);
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
                        mActionQueue.Enqueue(new MoveNode(selectedTile.GetPosition()));
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
                        ISkillAction decoratedSkill = BulletFactory.ApplyBulletEffect(mSelectedBullets, baseSkill);
                        mPreparedSkill = decoratedSkill;
                        mActionQueue.Enqueue(new AttackNode(decoratedSkill, mSelectedSkill, mSelectedTarget));
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
    private void HandleMoveSelected()
    {
        mActionQueue.Enqueue(new WaitInputNode(mPlayerEntity));
        BattleManager.Instance.BroadCastTurnInfo("Select tile to move");
    }
    private void HandleCylinderSpin(List<EBulletType> bullets) 
    {
        mSelectedBullets = bullets;
        mActionQueue.Enqueue(new TargetSelectNode(mSelectedSkill));
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
                    Events.RaiseOpenCylinderUI();

                    mActionQueue.Enqueue(new WaitNode());
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
        Events.OnMoveSelected -= HandleMoveSelected;
        Events.OnSkillSelected -= HandleSkillSelected;
        Events.OnCylinderSpin -= HandleCylinderSpin;
        Events.OnTurnSkip -= HandleTurnSkip;
    }
}
