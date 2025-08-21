using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SnakeHead))]
public class SnakeSpeedControl : MonoBehaviour
{
    [SerializeField] private float _minSpeed = 0f;
    [SerializeField] private int _thresholdSlowdown = 5;
    [SerializeField] private int _minRoadCountToEnd = 3;

    private SnakeHead _snakeHead;
    private Coroutine _coroutine;
    private float _initialSpeed;

    private void Awake()
    {
        _snakeHead = GetComponent<SnakeHead>();
        _initialSpeed = _snakeHead.Speed;
    }

    private void OnDisable()
    {
        EndControl();
    }

    private void Start()
    {
        _coroutine ??= StartCoroutine(ControlSpeed());
    }

    private void EndControl()
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }
    }

    private IEnumerator ControlSpeed()
    {
        bool isWork = true;
        bool isSlowingDown = false;
        float slowdownStartDistance = 0;

        while (isWork)
        {
            if (_snakeHead.CompareRoadPoint(_minRoadCountToEnd, _thresholdSlowdown, out float distanceToEnd))
            {
                if (isSlowingDown == false)
                {
                    isSlowingDown = true;
                    slowdownStartDistance = distanceToEnd;
                }

                float progress = 1f - (distanceToEnd / slowdownStartDistance);

                float newSpeed = Mathf.Lerp(_initialSpeed, _minSpeed, progress);
                _snakeHead.ChangeSpeed(newSpeed);

                if (distanceToEnd <= 0.1f || newSpeed <= _minSpeed + 0.1f)
                {
                    _snakeHead.ChangeSpeed(_minSpeed);
                    isSlowingDown = false;
                }
            }
            else if (isSlowingDown)
            {
                isSlowingDown = false;
                _snakeHead.ChangeSpeed(_initialSpeed);
            }

            yield return null;
        }
    }
}