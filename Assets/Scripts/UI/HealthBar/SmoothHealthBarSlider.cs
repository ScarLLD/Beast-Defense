using UnityEngine.UI;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Slider))]

public class SmoothHealthBarSlider : HealthBarSlider
{
    [SerializeField] private float _speed;

    private Coroutine _changeSliderCoroutine;
    private float _currentHealthPercentage;

    public override void OnAmountChanged(float currentHealth, float maxHealth)
    {
        if (_changeSliderCoroutine != null)
        {
            StopCoroutine(_changeSliderCoroutine);
        }

        _changeSliderCoroutine = StartCoroutine(ChangeSlider
            (currentHealth, maxHealth));
    }

    private IEnumerator ChangeSlider(float currentHealth, float maxHealth)
    {
        _currentHealthPercentage = currentHealth / maxHealth;

        while (_slider.value != _currentHealthPercentage)
        {
            _slider.value = Mathf.MoveTowards(_slider.value, 
                _currentHealthPercentage, _speed * 
                Time.deltaTime);

            yield return null;
        }
    }
}
