using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class CylinderUI : MonoBehaviour, IDragHandler, IEndDragHandler
{
    public Transform cylinderTransform;
    public List<Image> bulletSlots;
    public List<Sprite> bulletImages;
    private float mRotationSpeed;
    private RevolverSylinder mCylinder;
    private bool mIsSpinning = false;

    public GameObject mEffectInfoPanel;
    public TextMeshProUGUI mEffectTExt;

    private WaitForSeconds mWaitForSeconds = new WaitForSeconds(0.9f);

    [Header("EventChannel")]
    [SerializeField] private GameEventChannelSO mEventChannel;

    private void Awake()
    {
        mEventChannel.OnEventRaised += HandleGameEvent;
        gameObject.SetActive(false);
        mEffectInfoPanel.SetActive(false);
    }
    private void OnDestroy()
    {
        mEventChannel.OnEventRaised -= HandleGameEvent;
    }

    private void HandleGameEvent(EGameEventType type, object payload) 
    {
        if (type == EGameEventType.OpenCylinderUI) 
        {
            gameObject.SetActive(true);
            ReloadCylinder();
        }
    }

    public void ReloadCylinder() 
    {
        mCylinder = UpgradeManager.Instance.revolverCylinder;
        mCylinder.Reload();

        EBulletType[] bullets = mCylinder.GetBullets();
        for (int i = 0; i < bulletSlots.Count; i++) 
        {
            bulletSlots[i].sprite = bulletImages[(int)bullets[i]];
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        mRotationSpeed = eventData.delta.x;
        cylinderTransform.Rotate(Vector3.forward, mRotationSpeed);

        if (!mIsSpinning) 
        {
            mIsSpinning = true;
            mEventChannel.RaiseEvent(EGameEventType.CylinderSpinStart);
        }
    }

    public void OnEndDrag(PointerEventData eventData) 
    {
        StartCoroutine(SlowDownCo());
        mIsSpinning = false;
    }

    private IEnumerator SlowDownCo() 
    {
        while (Mathf.Abs(mRotationSpeed) > 0.1f) 
        {
            cylinderTransform.Rotate(Vector3.forward, mRotationSpeed);
            mRotationSpeed = Mathf.Lerp(mRotationSpeed, 0, Time.deltaTime * 5f);
            yield return null;
        }
        //°¢µµ °è»ê
        float angle = cylinderTransform.eulerAngles.z;
        float snapAngle = Mathf.Round(angle / 60f) * 60f;
        //»ìÂ¦ Ã¶ÄÀÇÏ´Â È¿°ú
        float duration = 0.3f;
        float elapsed = 0f;
        float startAngle = angle;

        while (elapsed < duration) 
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float eased = Mathf.Sin(t * Mathf.PI * 0.5f);
            float currentAngle = Mathf.Lerp(startAngle, snapAngle, eased);
            cylinderTransform.rotation = Quaternion.Euler(0, 0, currentAngle);
            yield return null;
        }

        cylinderTransform.rotation = Quaternion.Euler(0, 0, snapAngle);

        int selectedIndex = GetTopSlotIndex();
        EBulletType selectedBullet = mCylinder.GetBullets()[selectedIndex];

        mEffectInfoPanel.SetActive(true);
        SetEffectText(selectedBullet);

        List<EBulletType> bullets = new List<EBulletType>();
        bullets.Add(selectedBullet);

        var paload = new CylinderSpinPayload { bullets = new List<EBulletType> { selectedBullet } };
        mEventChannel.RaiseEvent(EGameEventType.CylinderSpinEnd, paload);

        yield return mWaitForSeconds;

        mEffectInfoPanel.SetActive(false);
        gameObject.SetActive(false);
    }

    private void SetEffectText(EBulletType bullet) 
    {
        switch (bullet) 
        {
            case EBulletType.Normal:
                mEffectTExt.text = "Normal Bullet";
                break;
            case EBulletType.Critical:
                mEffectTExt.text = "Critical Bullet";
                break;
            case EBulletType.Heal:
                mEffectTExt.text = "Heal Bullet";
                break;
        }
    }

    private int GetTopSlotIndex() 
    {
        float angle = cylinderTransform.eulerAngles.z;
        int index = (int)(Mathf.Round(angle / 60f)) % 6;
        return index;
    }
}
