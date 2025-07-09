using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SnakeHead))]
public class SnakeSpeedControl : MonoBehaviour
{
    [SerializeField] private float _minSpeed = 0f;          // ћинимальна€ скорость (0 дл€ полной остановки)
    [SerializeField] private int _thresholdSlowdown = 5;    // ƒистанци€, на которой начинаем замедл€тьс€
    [SerializeField] private int _minRoadCountToEnd = 3;

    private SnakeHead _snakeHead;
    private float _initialSpeed;
    private bool _isSlowingDown = false;
    private float _slowdownStartDistance;

    private void Awake()
    {
        _snakeHead = GetComponent<SnakeHead>();
        _initialSpeed = _snakeHead.Speed;
    }

    private IEnumerator ControlSpeed()
    {
        while (true)
        {
            if (_snakeHead.CompareRoadPoint(_minRoadCountToEnd, _thresholdSlowdown, out float distanceToEnd))
            {
                if (!_isSlowingDown)
                {
                    _isSlowingDown = true;
                    _slowdownStartDistance = distanceToEnd;
                }

                // Ќормализуем оставшеес€ рассто€ние (от 1 до 0)
                float progress = 1f - (distanceToEnd / _slowdownStartDistance);

                // ѕлавно уменьшаем скорость (можно заменить на SmoothStep дл€ более м€гкого замедлени€)
                float newSpeed = Mathf.Lerp(_initialSpeed, _minSpeed, progress);
                _snakeHead.ChangeSpeed(newSpeed);

                // ≈сли достигли конца или почти остановились
                if (distanceToEnd <= 0.1f || newSpeed <= _minSpeed + 0.1f)
                {
                    _snakeHead.ChangeSpeed(_minSpeed);
                    _isSlowingDown = false;
                }
            }
            else if (_isSlowingDown)
            {
                _isSlowingDown = false;
                _snakeHead.ChangeSpeed(_initialSpeed);
            }

            yield return null;
        }
    }

    private void Start()
    {
        StartCoroutine(ControlSpeed());
    }
}