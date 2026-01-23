using UnityEngine;
using TMPro;

public class DamagePopUpUI : MonoBehaviour
{
    public float moveUpSpeed = 1f;
    public float fadeOutSpeed = 2f;
    [SerializeField] private TextMeshProUGUI mDamageText;
    private Color mOriginColor;

    private void Awake()
    {
        if (mDamageText == null) mDamageText = GetComponentInChildren<TextMeshProUGUI>();
        mOriginColor = mDamageText.color;
    }

    public void SetDamage(int damage) 
    {
        mDamageText.text = damage.ToString("F0");
        mDamageText.color = mOriginColor;
    }

    private void Update()
    {
        transform.Translate(Vector3.up * moveUpSpeed * Time.deltaTime);

        transform.LookAt(Camera.main.transform);
        transform.Rotate(0, 180, 0); //뒤집힘 방지

        Color c = mDamageText.color;
        c.a -= fadeOutSpeed * Time.deltaTime;
        mDamageText.color = c;

        if (mDamageText.color.a <= 0) 
        {
            PoolManager.Instance.ReturnPool(this);
        }
    }
}
