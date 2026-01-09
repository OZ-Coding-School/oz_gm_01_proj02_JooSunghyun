
public class PlayerTurnStateMachine : TurnStateMachine
{
    private Entity mPlayerEntity;
    private SkillSO mSelectedSkill;
    private Entity mSelectedTarget;

    public PlayerTurnStateMachine(Entity entity) : base(entity)
    {
        mPlayerEntity = entity;
    }

    public override void StartTurn()
    {
        mActionQueue.Enqueue(new WaitInputNode(mPlayerEntity));
        //이벤트 발행
        BattleManager.Instance.BroadCastTurnInfo("Select tile to move");
        BattleManager.Instance.BroadCastSkillUIInfo(mPlayerEntity.GetUnitData().skills);
    }

    public override void Update()
    {
        if (mActionQueue.Count > 0)
        {
            ActionNode currentNode = mActionQueue.Peek();

            if (currentNode.Evaluate(mPlayerEntity))
            {
                mActionQueue.Dequeue();
                if (currentNode is WaitInputNode waitInputNode) 
                {
                    TileBase selectedTile = waitInputNode.GetSelectedTile();
                    if (selectedTile != null) 
                    {
                        BattleManager.Instance.BroadCastTurnInfo("Moving...");
                        mActionQueue.Enqueue(new MoveNode(selectedTile.GetPosition()));
                    }
                }
                else if (currentNode is MoveNode)
                {
                    mActionQueue.Enqueue(new SkillSelectNode(mPlayerEntity));
                    BattleManager.Instance.BroadCastTurnInfo("Select skill to use");
                }
                else if (currentNode is SkillSelectNode skillNode)
                {
                    mSelectedSkill = skillNode.GetSelectedSkill();
                    skillNode.Dispose();
                    mActionQueue.Enqueue(new TargetSelectNode(mSelectedSkill));
                    BattleManager.Instance.BroadCastTurnInfo("Select target");
                }
                else if (currentNode is TargetSelectNode targetNode)
                {
                    mSelectedTarget = targetNode.GetSelectedTarget();
                    if (mSelectedSkill != null && mSelectedTarget != null)
                    {
                        mActionQueue.Enqueue(new AttackNode(mSelectedSkill, mSelectedTarget));
                    }
                    mActionQueue.Enqueue(new EndTurnNode());
                    BattleManager.Instance.BroadCastTurnInfo("End turn");
                }
            }
        }
    }
}
