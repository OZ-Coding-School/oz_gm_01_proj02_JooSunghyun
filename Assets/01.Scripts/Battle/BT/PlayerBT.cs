
using System.Collections.Generic;

public class PlayerBT
{
    private List<ActionNode> actions = new List<ActionNode>();

    public void AddAction(ActionNode node) 
    {
        actions.Add(node);
    }

    public void Evaluate(Entity entity) 
    {
        foreach (var action in actions) 
        {
            if (action.Evaluate(entity)) break;
        }
    }

    public List<ActionNode> GetActionList() 
    {
        return actions; 
    }
}
