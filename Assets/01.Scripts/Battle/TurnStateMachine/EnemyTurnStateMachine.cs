using UnityEngine;

public class EnemyTurnStateMachine : TurnStateMachine
{
    private Entity mCurrEntity;
    private SkillSO selectedSkill;
    private Entity selectedTarget;

    public EnemyTurnStateMachine(Entity entity) : base(entity)
    {
        mCurrEntity = entity;
    }

    public override void StartTurn()
    {
        Debug.Log("EnemyTurn");
        mActionQueue.Enqueue(new WaitInputNode(mCurrEntity));
    }

    public override void Update()
    {
        if (mActionQueue.Count > 0)
        {
            ActionNode currentNode = mActionQueue.Peek();

            if (currentNode.Evaluate(mCurrEntity))
            {
                mActionQueue.Dequeue();
                if (currentNode is WaitInputNode waitInputNode)
                {
                    Debug.Log("WaitInputNode");
                    TileBase selectedTile = waitInputNode.GetSelectedTile();
                    if (selectedTile != null)
                    {
                        mActionQueue.Enqueue(new MoveNode(selectedTile.GetPosition()));
                    }
                }
                else if (currentNode is MoveNode)
                {
                    Debug.Log("MoveNode");
                    mActionQueue.Enqueue(new SkillSelectNode(mCurrEntity));
                }
                else if (currentNode is SkillSelectNode skillNode)
                {
                    Debug.Log("SkillSelectNode");
                    selectedSkill = skillNode.GetSelectedSkill();
                    skillNode.Dispose();
                    mActionQueue.Enqueue(new TargetSelectNode());
                }
                else if (currentNode is TargetSelectNode targetNode)
                {
                    Debug.Log("TargetSelectNode");
                    selectedTarget = targetNode.GetSelectedTarget();
                    if (selectedSkill != null && selectedTarget != null)
                    {
                        mActionQueue.Enqueue(new AttackNode(selectedSkill, selectedTarget));
                    }
                    mActionQueue.Enqueue(new EndTurnNode());
                }
            }
        }
    }
}
