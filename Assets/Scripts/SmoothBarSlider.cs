using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SmoothBarSlider : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private Slider _slider;

    private readonly float _maxSliderValue = 1;
    private float _currentBarPercentage;
    private Coroutine _changeSliderCoroutine;
    private Snake _snake;

    public void Init(Snake snake)
    {
        if (_snake == null)
        {
            _snake = snake;
            _snake.SegmentsCountChanged += OnCountChanged;
        }

        SetDefaultValue();
    }

    private void OnEnable()
    {
        if (_snake != null)
            _snake.SegmentsCountChanged += OnCountChanged;
    }

    private void OnDisable()
    {
        if (_snake != null)
            _snake.SegmentsCountChanged -= OnCountChanged;
    }

    private void OnCountChanged(float currentCount, float maxCount)
    {
        if (_changeSliderCoroutine != null)
        {
            StopCoroutine(_changeSliderCoroutine);
        }

        _changeSliderCoroutine = StartCoroutine(ChangeSlider
            (currentCount, maxCount));
    }
    private void SetDefaultValue()
    {
        _slider.maxValue = _maxSliderValue;
        _slider.minValue = 0;
    }

    private IEnumerator ChangeSlider(float currentCount, float maxCount)
    {
        _currentBarPercentage = currentCount / maxCount;

        while (_slider.value != _currentBarPercentage)
        {
            _slider.value = Mathf.MoveTowards(_slider.value,
                _currentBarPercentage, _speed *
                Time.deltaTime);

            yield return null;
        }
    }
}
