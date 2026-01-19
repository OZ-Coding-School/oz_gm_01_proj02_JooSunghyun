using System.Collections.Generic;
using UnityEngine;
public enum EBGMType { Lobby, Battle, Victory, Defeat }
public enum ESFXType { Move, Attack, Hit, TurnStart, TurnEnd, Button }

[System.Serializable]
public class BGMEntry
{
    public EBGMType type;
    public AudioClip clip;
}
[System.Serializable]
public class SFXEntry
{
    public ESFXType type;
    public AudioClip clip;
}
[CreateAssetMenu(fileName = "SoundDatabase", menuName = "Scriptable Objects/SoundDatabase")]
public class SoundDatabase : ScriptableObject
{
    public List<BGMEntry> bgmEntries;
    public List<SFXEntry> sfxEntries;

    private Dictionary<EBGMType, AudioClip> mBGMDictionary;
    private Dictionary<ESFXType, AudioClip> mSFXDictionary;

    public void Init() 
    {
        mBGMDictionary = new Dictionary<EBGMType, AudioClip> ();
        foreach (var entry in bgmEntries) 
        {
            if (!mBGMDictionary.ContainsKey(entry.type)) 
            {
                mBGMDictionary.Add(entry.type, entry.clip);
            }
        }

        mSFXDictionary = new Dictionary<ESFXType, AudioClip>();
        foreach (var entry in sfxEntries)
        {
            if (!mSFXDictionary.ContainsKey(entry.type))
            {
                mSFXDictionary.Add(entry.type, entry.clip);
            }
        }
    }
    public AudioClip GetBGM(EBGMType type) 
    {
        if (mBGMDictionary == null) Init();
        mBGMDictionary.TryGetValue(type, out var clip);
        return clip;
    }
    public AudioClip GetSFX(ESFXType type)
    {
        if (mSFXDictionary == null) Init();
        mSFXDictionary.TryGetValue(type, out var clip);
        return clip;
    }
}
