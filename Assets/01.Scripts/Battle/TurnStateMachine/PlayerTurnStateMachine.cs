
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
        UnityEngine.Debug.Log("PlayerTurn");
        mActionQueue.Enqueue(new WaitInputNode(mPlayerEntity));
        //이벤트 발행
        BattleEvents.RaiseSkillUIUpdate(mPlayerEntity.GetUnitData().skills);
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
                    UnityEngine.Debug.Log("WaitInputNode");
                    TileBase selectedTile = waitInputNode.GetSelectedTile();
                    if (selectedTile != null) 
                    {
                        mActionQueue.Enqueue(new MoveNode(selectedTile.GetPosition()));
                    }
                }
                else if (currentNode is MoveNode)
                {
                    UnityEngine.Debug.Log("MoveNode");
                    mActionQueue.Enqueue(new SkillSelectNode(mPlayerEntity));
                }
                else if (currentNode is SkillSelectNode skillNode)
                {
                    UnityEngine.Debug.Log("SkillSelectNode");
                    mSelectedSkill = skillNode.GetSelectedSkill();
                    skillNode.Dispose();
                    mActionQueue.Enqueue(new TargetSelectNode());
                }
                else if (currentNode is TargetSelectNode targetNode)
                {
                    UnityEngine.Debug.Log("TargetSelectNode");
                    mSelectedTarget = targetNode.GetSelectedTarget();
                    if (mSelectedSkill != null && mSelectedTarget != null)
                    {
                        mActionQueue.Enqueue(new AttackNode(mSelectedSkill, mSelectedTarget));
                    }
                    mActionQueue.Enqueue(new EndTurnNode());
                }
            }
        }
    }
}
