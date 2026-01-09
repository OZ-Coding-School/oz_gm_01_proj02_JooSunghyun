using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider mSlider;

    private void LateUpdate()
    {
        transform.LookAt(Camera.main.transform);
    }

    public void SetMaxHealth(float maxHP) 
    {
        mSlider.maxValue = maxHP;
        SetHealth(maxHP);
    }
    public void SetHealth(float hp) 
    {
        mSlider.value = hp;
    }
}
