using UnityEngine;

public abstract class SkillModifier
{
    public string Name { get; protected set; }
    public string Description { get; protected set; }

    public abstract void Register(Entity entity);
}
