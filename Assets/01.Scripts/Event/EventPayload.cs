using UnityEngine;
using System.Collections.Generic;

public class TurnEventPayload
{
    public Entity entity;
}

public class MoveEventPayload
{
    public Entity entity;
    public int movedDistance;
}

public class SkillUsedPayload
{
    public SkillSO skill;
    public Entity target;
}

public class TargetSelectedPayload
{
    public Entity caster;
    public Entity target;
}

public class DamagePayload
{
    public float damage;
    public Entity attacker;
    public Entity target;
}

public struct SkillSelectedPayload
{
    public int skillIndex;
}

public class SkillUIUpdatePayload
{
    public List<SkillSO> skills;
}

public struct TurnInfoPayload
{
    public string info;
}

public struct StageChangePayload
{
    public int stageLevel;
}

public struct APUpdatePayload
{
    public int ap;
}

public class EntityDiedPayload
{
    public Entity entity;
}

public class CylinderSpinPayload
{
    public List<EBulletType> bullets;
}

public class LevelUpPayload
{
    public int newLevel;
    public List<UpgradeSO> choices;
}

public class UpgradeSelectedPayload
{
    public UpgradeSO selected;
}

public class PlayerSpawnedPayload 
{
    public Entity entity;
}

public class VolumeUpdatePayload 
{
    public float volume;
}

public class MutePayload 
{
    public bool isMuted;
}