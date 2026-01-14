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

    public GameObject mEffectInfoPanel;
    public TextMeshProUGUI mEffectTExt;

    private WaitForSeconds mWaitForSeconds = new WaitForSeconds(0.9f);
    private void Awake()
    {
        Events.OnOpenCylinderUI += HandleOpenCylinderUI;
        gameObject.SetActive(false);
        mEffectInfoPanel.SetActive(false);
    }
    private void OnDestroy()
    {
        Events.OnOpenCylinderUI -= HandleOpenCylinderUI;
    }

    private void HandleOpenCylinderUI() 
    {
        gameObject.SetActive(true);
        ReloadCylinder();
    }

    public void ReloadCylinder() 
    {
        mCylinder = GameManager.Instance.revolverCylinder;
        mCylinder.Reload();

        EBulletType[] bullets = mCylinder.GetBullets();
        Debug.Log(bulletSlots.Count);
        for (int i = 0; i < bulletSlots.Count; i++) 
        {
            Debug.Log("enum count"+(int)bullets[i]);
            bulletSlots[i].sprite = bulletImages[(int)bullets[i]];
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        mRotationSpeed = eventData.delta.x;
        cylinderTransform.Rotate(Vector3.forward, mRotationSpeed);
    }

    public void OnEndDrag(PointerEventData eventData) 
    {
        StartCoroutine(SlowDownCo());
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
        SetEffevtText(selectedBullet);
        yield return mWaitForSeconds;

        Events.RaiseCylinderSpin(selectedBullet);
        mEffectInfoPanel.SetActive(false);
        gameObject.SetActive(false);
    }

    private void SetEffevtText(EBulletType bullet) 
    {
        switch (bullet) 
        {
            case EBulletType.Normal:
                mEffectTExt.text = "Normal Bullet";
                break;
            case EBulletType.Critical:
                mEffectTExt.text = "Damage X 2";
                break;
            case EBulletType.Heal:
                mEffectTExt.text = "HP +20";
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
