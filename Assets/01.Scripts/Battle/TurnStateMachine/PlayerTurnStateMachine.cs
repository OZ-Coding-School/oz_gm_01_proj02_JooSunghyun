
using System.Diagnostics;

public class PlayerTurnStateMachine : TurnStateMachine
{
    private Entity mPlayerEntity;
    private SkillSO mSelectedSkill;
    private Entity mSelectedTarget;
    private EBulletType mSelectedBullet;
    private RevolverSylinder mCylinder;
    private BulletEffectHandler mBulletHandler;

    public PlayerTurnStateMachine(Entity entity) : base(entity)
    {
        mPlayerEntity = entity;
        mCylinder = GameManager.Instance.revolverCylinder;
        mBulletHandler = new BulletEffectHandler();

        Events.OnMoveSelected += HandleMoveSelected;
        Events.OnSkillSelected += HandleSkillSelected;
        Events.OnCylinderSpin += HandleCylinderSpin;
        Events.OnTurnSkip += HandleTurnSkip;
    }

    public override void StartTurn()
    {
        mPlayerEntity.currUnitAP += mPlayerEntity.GetUnitData().unitAP;
     
        BattleManager.Instance.BroadCastTurnInfo("Player Turn");
        BattleManager.Instance.BroadCastSkillUIInfo(mPlayerEntity.GetUnitData().skills);
        Events.RaiseAPUpdate(mPlayerEntity.currUnitAP);
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
                        mBulletHandler.ApplyEffect(mSelectedBullet, mPlayerEntity, mSelectedTarget);
                        mActionQueue.Enqueue(new AttackNode(mSelectedSkill, mSelectedTarget));
                    }
                    else
                    {
                        AfterAction();
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
    private void HandleCylinderSpin(EBulletType t) 
    {
        mSelectedBullet = t;
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
        UnityEngine.Debug.Log("After Action");
        Events.RaiseAPUpdate(mPlayerEntity.currUnitAP);
        if (mPlayerEntity.currUnitAP <= 0)
        {
            UnityEngine.Debug.Log("End Turn");
            mActionQueue.Enqueue(new EndTurnNode());
        }
        else 
        {
            UnityEngine.Debug.Log("Wait Action");
            mActionQueue.Enqueue(new WaitNode());
            BattleManager.Instance.BroadCastTurnInfo("Choose your action");
            BattleManager.Instance.BroadCastSkillUIInfo(mPlayerEntity.GetUnitData().skills);
        }
    }

    public override void Dispose()
    {
        Events.OnMoveSelected -= HandleMoveSelected;
        Events.OnSkillSelected -= HandleSkillSelected;
        Events.OnTurnSkip -= HandleTurnSkip;
    }
}
