using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeSO", menuName = "Scriptable Objects/UpgradeSO")]
public class UpgradeSO : ScriptableObject
{
    public string upgradeName;
    public string description;
    public Sprite icon;

    public EUpgradeEffectType effectType;
    public float value;
}
