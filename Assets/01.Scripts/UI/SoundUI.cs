using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SoundUI : MonoBehaviour
{
    [SerializeField] private Slider mBGMSlider;
    [SerializeField] private Slider mSFXSlider;

    [SerializeField] private Image mBGMButtonImage;
    [SerializeField] private Image mSFXButtonImage;

    [SerializeField] private Sprite mMuteIcon;
    [SerializeField] private Sprite mUnMuteIcon;

    [SerializeField] private TextMeshProUGUI mBGMVolumeText;
    [SerializeField] private TextMeshProUGUI mSFXVolumeText;

    [SerializeField] private GameEventChannelSO mEventChannel;

    private bool mBGMMuted = false;
    private bool mSFXMuted = false;

    private void Start()
    {
        if (SoundManager.Instance != null) 
        {
            mBGMSlider.value = SoundManager.Instance.GetBGMVolume();
            mSFXSlider.value = SoundManager.Instance.GetSFXVolume();
        }

        mBGMSlider.onValueChanged.AddListener(OnBGMVolumeChanged);
        mSFXSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
    }

    private void OnBGMVolumeChanged(float value) 
    {
        var payload = new VolumeUpdatePayload { volume = value };
        mEventChannel.RaiseEvent(EGameEventType.VolumeBGMUpdate, payload);
        mBGMVolumeText.text = (value * 100f).ToString("F0") + "%";
    }
    private void OnSFXVolumeChanged(float value)
    {
        var payload = new VolumeUpdatePayload { volume = value };
        mEventChannel.RaiseEvent(EGameEventType.VolumeSFXUpdate, payload);
        mSFXVolumeText.text = (value * 100f).ToString("F0") + "%";
    }

    public void ToggleBGMMute() 
    {
        mBGMMuted = !mBGMMuted;
        var payload = new MutePayload { isMuted = mBGMMuted };
        mEventChannel.RaiseEvent(EGameEventType.VolumeBGMMuteToggle, payload);
        UpdateButtonImage(mBGMButtonImage, 0);
    }
    public void ToggleSFXMute()
    {
        mSFXMuted = !mSFXMuted;
        var payload = new MutePayload { isMuted = mSFXMuted };
        mEventChannel.RaiseEvent(EGameEventType.VolumeSFXMuteToggle, payload);
        UpdateButtonImage(mSFXButtonImage, 1);
    }

    private void UpdateButtonImage(Image buttonImage, int k) 
    {
        bool isMuted = true;
        if (k == 0) isMuted = mBGMMuted;
        if (k == 1) isMuted = mSFXMuted;
        buttonImage.sprite = isMuted ? mMuteIcon : mUnMuteIcon;
    }
}
