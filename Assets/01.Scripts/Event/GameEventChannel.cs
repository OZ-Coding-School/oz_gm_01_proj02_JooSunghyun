using UnityEngine;
using UnityEngine.Events;

public enum EGameEventType 
{
    TurnStart,
    TurnEnd,
    MoveStart,
    MoveEnd,
    MoveSelected,
    SkillUsed,
    TargetSelected,
    DamageDealt,
    SkillSelected,
    SkillUIUpdate,
    TurnInfoUpdate,
    OpenCylinderUI,
    TurnSkip,
    StageChange,
    APUpdate,
    EntityDied,
    CylinderSpinEnd,
    LevelUp,
    UpgradeSelected,
    PlayerSpawned,
    ButtonClicked,
    CylinderSpinStart,
    VolumeBGMUpdate,
    VolumeSFXUpdate,
    VolumeBGMMuteToggle,
    VolumeSFXMuteToggle
}

[CreateAssetMenu(fileName = "GameEventChannel", menuName = "Scriptable Objects/GameEventChannel")]
public class GameEventChannelSO : ScriptableObject
{
    public UnityAction<EGameEventType, object> OnEventRaised;

    public void RaiseEvent(EGameEventType type, object payload = null) 
    {
        OnEventRaised?.Invoke(type, payload);
    }

    public void RaiseButtonClicked() 
    {
        RaiseEvent(EGameEventType.ButtonClicked);
    }
}
