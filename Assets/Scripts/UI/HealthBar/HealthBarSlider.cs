using UnityEngine.UI;
using UnityEngine;

[RequireComponent(typeof(Slider))]

public class HealthBarSlider : HealthBar
{    
    protected Slider _slider;
    protected float _maxSliderValue = 1;

    protected void Awake()
    {
        _slider = GetComponent<Slider>();

        SetDefaultValue();
    }
   
    public override void OnAmountChanged(float currentHealth , float maxHealth)
    {
        float currentHealthPercentage = currentHealth / maxHealth;

        _slider.value = currentHealthPercentage;
    }

    protected void SetDefaultValue()
    {
        _slider.maxValue = _maxSliderValue;
        _slider.minValue = 0;
    }
}
