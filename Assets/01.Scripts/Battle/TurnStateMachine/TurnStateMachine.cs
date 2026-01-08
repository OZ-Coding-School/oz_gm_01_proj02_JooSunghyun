using System.Collections.Generic;

public abstract class TurnStateMachine
{
    protected Queue<ActionNode> mActionQueue = new Queue<ActionNode>();
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
            ActionNode currentNode = mActionQueue.Peek();

            if (currentNode.Evaluate(mCurrentEntity)) 
            {
                mActionQueue.Dequeue();
            }
        }
    }
}
