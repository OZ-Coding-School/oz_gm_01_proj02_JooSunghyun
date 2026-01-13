using System.Collections.Generic;

public abstract class TurnStateMachine : IDisposable
{
    protected Queue<BTNode> mActionQueue = new Queue<BTNode>();
    protected Entity mCurrentEntity;
    public bool IsFinished => mActionQueue.Count == 0;

    public TurnStateMachine(Entity entity) 
    {
        mCurrentEntity = entity;
    }

    public abstract void StartTurn();

    public virtual void Update() 
    {
        if (mActionQueue.Count > 0) 
        {
            BTNode currentNode = mActionQueue.Peek();

            if (currentNode.Evaluate(mCurrentEntity)) 
            {
                mActionQueue.Dequeue();
            }
        }
    }

    protected bool HasEnoughCost(Entity entity, SkillSO skill)
    {
        if (entity.TryGetComponent(out Entity e)) 
        {
            if (e.currUnitAP >= skill.skillCost) return true;
            else return false;
        }
        return false;
    }

    public virtual void Dispose() { }
}
