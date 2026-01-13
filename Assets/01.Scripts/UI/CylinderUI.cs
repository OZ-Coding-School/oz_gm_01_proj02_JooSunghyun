using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class CylinderUI : MonoBehaviour, IDragHandler, IEndDragHandler
{
    public Transform cylinderTransform;
    public List<Image> bulletSlots;
    public List<Sprite> bulletImages;
    private float mRotationSpeed;
    private RevolverSylinder mCylinder;

    private void Awake()
    {
        Events.OnOpenCylinderUI += HandleOpenCylinderUI;
        gameObject.SetActive(false);
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

        Events.RaiseCylinderSpin(selectedBullet);
        gameObject.SetActive(false);
    }

    private int GetTopSlotIndex() 
    {
        float angle = cylinderTransform.eulerAngles.z;
        int index = (int)(Mathf.Round(angle / 60f)) % 6;
        return index;
    }
}
