using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [SerializeField] private AudioSource mBGMSource;
    [SerializeField] private AudioSource mSFXSourcePrefab;
    [SerializeField] private SoundDatabase mDatabase;

    private List<AudioSource> mSFXPool = new List<AudioSource>();
    private WaitForSeconds mWaitForSeconds = new WaitForSeconds(0.5f);

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitPool();
            mDatabase.Init();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void InitPool() 
    {
        for (int i = 0; i < 10; i++) 
        {
            AudioSource source = Instantiate(mSFXSourcePrefab, transform);
            mSFXPool.Add(source);
        }
    }
    private AudioSource GetSource() 
    {
        foreach (var src in mSFXPool) 
        {
            if (!src.isPlaying) return src;
        }
        return mSFXPool[0];
    }
    public void PlayBGM(EBGMType type) 
    {
        AudioClip clip = mDatabase.GetBGM(type);
        if (clip != null) 
        {
            StartCoroutine(FadeInBGM(clip));
        }      
    }
    public void PlaySFX(ESFXType type) 
    {
        AudioClip clip = mDatabase.GetSFX(type);
        if (clip != null)
        {
            StartCoroutine(FadeInBGM(clip));
        }
    }
    private IEnumerator FadeInBGM(AudioClip clip) 
    {
        //지금 플레이하는거 볼륨 점점 줄이고
        if (mBGMSource.isPlaying) 
        {
            for (float v = 1f; v >= 0; v -= 0.05f) 
            {
                mBGMSource.volume = v;
                yield return mWaitForSeconds;
            }
        }
        //다 줄이면 새로운거
        mBGMSource.clip = clip;
        mBGMSource.Play();
        for (float v = 0; v <= 1f; v += 0.05f) 
        {
            mBGMSource.volume = v;
            yield return mWaitForSeconds;
        }
    }
}
