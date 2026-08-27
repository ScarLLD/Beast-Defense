using Game.Scripts.SnakeCore;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI
{
    public class SmoothBarSlider : MonoBehaviour
    {
        private const float MAX_SLIDER_VALUE = 1;

        [SerializeField] private float _speed;
        [SerializeField] private Slider _slider;

        private Snake _snake;

        private void Awake()
        {
            _slider.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            if (_snake)
                _snake.SegmentsCountChanged += OnCountChanged;
        }

        private void OnDisable()
        {
            if (_snake)
                _snake.SegmentsCountChanged -= OnCountChanged;
        }

        public void Init(Snake snake)
        {
            if (!_snake)
            {
                _snake = snake;
                _snake.SegmentsCountChanged += OnCountChanged;
            }

            _slider.gameObject.SetActive(true);
            SetDefaultValue();
        }

        private void OnCountChanged(float currentCount, float maxCount)
        {
            _slider.value = 1 - currentCount / maxCount;
        }

        private void SetDefaultValue()
        {
            _slider.maxValue = MAX_SLIDER_VALUE;
            _slider.minValue = 0;

            _slider.value = _slider.minValue;
        }
    }
}