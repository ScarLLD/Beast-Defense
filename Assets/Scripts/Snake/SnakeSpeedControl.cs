using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Snake))]
public class SnakeSpeedControl : MonoBehaviour
{
    private Snake _snake;
    private Coroutine _controlCoroutine;
    private Coroutine _transitionCoroutine;
    private float _initialSpeed;
    private SpeedState _currentState = SpeedState.Normal;

    [SerializeField] private float _transitionDuration = 1f;

    [Header("Speed Multipliers")]
    [SerializeField] private float _slowedMultiplier = 0.5f;
    [SerializeField] private float _deepSlowedMultiplier = 0.25f;
    [SerializeField] private float _finalSlowdownMultiplier = 0.05f;

    [Header("Distance Thresholds (0..1)")]
    [SerializeField] private float _slowedDistance = 0.5f;
    [SerializeField] private float _deepSlowedDistance = 0.75f;
    [SerializeField] private float _finalSlowdownDistance = 0.9f;
    [SerializeField] private float _stopDistance = 0.99f;

    private void Awake()
    {
        _snake = GetComponent<Snake>();
    }

    private void OnDisable()
    {
        EndControl();
    }

    public void StartControl()
    {
        if (_controlCoroutine != null)
            StopCoroutine(_controlCoroutine);

        _controlCoroutine = StartCoroutine(ControlSpeed());
    }

    private void EndControl()
    {
        if (_controlCoroutine != null)
        {
            StopCoroutine(_controlCoroutine);
            _controlCoroutine = null;
        }

        if (_transitionCoroutine != null)
        {
            StopCoroutine(_transitionCoroutine);
            _transitionCoroutine = null;
        }
    }

    private IEnumerator ControlSpeed()
    {
        _initialSpeed = _snake.MoveSpeed;
        _currentState = SpeedState.Normal;

        while (_currentState != SpeedState.Stopped)
        {
            float distance = _snake.NormalizedPosition;

            switch (_currentState)
            {
                case SpeedState.Normal:
                    HandleNormalState(distance);
                    break;
                case SpeedState.Slowed:
                    HandleSlowedState(distance);
                    break;
                case SpeedState.DeepSlowed:
                    HandleDeepSlowedState(distance);
                    break;
                case SpeedState.FinalSlowdown:
                    HandleFinalSlowdownState(distance);
                    break;
            }

            yield return new WaitForSeconds(0.1f);
        }

        Debug.Log("Speed control ended - snake stopped");
        _controlCoroutine = null;
    }

    private void HandleNormalState(float distance)
    {
        if (distance >= _finalSlowdownDistance)
        {
            _currentState = SpeedState.FinalSlowdown;
            StartFinalSlowdown();
        }
        else if (distance >= _deepSlowedDistance)
        {
            _currentState = SpeedState.DeepSlowed;
            StartSpeedTransition(_initialSpeed * _deepSlowedMultiplier);
        }
        else if (distance >= _slowedDistance)
        {
            _currentState = SpeedState.Slowed;
            StartSpeedTransition(_initialSpeed * _slowedMultiplier);
        }
    }

    private void HandleSlowedState(float distance)
    {
        if (distance >= _finalSlowdownDistance)
        {
            _currentState = SpeedState.FinalSlowdown;
            StartFinalSlowdown();
        }
        else if (distance >= _deepSlowedDistance)
        {
            _currentState = SpeedState.DeepSlowed;
            StartSpeedTransition(_initialSpeed * _deepSlowedMultiplier);
        }
        else if (distance < _slowedDistance)
        {
            _currentState = SpeedState.Normal;
            StartSpeedTransition(_initialSpeed);
        }
    }

    private void HandleDeepSlowedState(float distance)
    {
        if (distance >= _finalSlowdownDistance)
        {
            _currentState = SpeedState.FinalSlowdown;
            StartFinalSlowdown();
        }
        else if (distance < _deepSlowedDistance && distance >= _slowedDistance)
        {
            _currentState = SpeedState.Slowed;
            StartSpeedTransition(_initialSpeed * _slowedMultiplier);
        }
        else if (distance < _slowedDistance)
        {
            _currentState = SpeedState.Normal;
            StartSpeedTransition(_initialSpeed);
        }
    }

    private void HandleFinalSlowdownState(float distance)
    {
        if (distance >= _stopDistance)
        {
            _currentState = SpeedState.Stopped;
            _snake.ChangeSpeed(0f);
        }
        else if (distance < _finalSlowdownDistance && distance >= _deepSlowedDistance)
        {
            _currentState = SpeedState.DeepSlowed;
            StartSpeedTransition(_initialSpeed * _deepSlowedMultiplier);
        }
        else if (distance < _slowedDistance)
        {
            _currentState = SpeedState.Normal;
            StartSpeedTransition(_initialSpeed);
        }
    }

    private void StartFinalSlowdown()
    {
        if (_transitionCoroutine != null)
            StopCoroutine(_transitionCoroutine);

        float remainingDistance = 1f - _snake.NormalizedPosition;
        float duration = Mathf.Max(2f, remainingDistance * 3f);

        _transitionCoroutine = StartCoroutine(FinalSlowdownCoroutine(duration));
    }

    private void StartSpeedTransition(float targetSpeed)
    {
        if (_transitionCoroutine != null)
            StopCoroutine(_transitionCoroutine);

        _transitionCoroutine = StartCoroutine(SpeedTransitionCoroutine(targetSpeed, _transitionDuration));
    }

    private IEnumerator FinalSlowdownCoroutine(float duration)
    {
        float startSpeed = _snake.MoveSpeed;
        float elapsed = 0f;
        float targetSpeed = _initialSpeed * _finalSlowdownMultiplier;

        while (elapsed < duration && _currentState == SpeedState.FinalSlowdown)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float newSpeed = Mathf.Lerp(startSpeed, targetSpeed, t);
            _snake.ChangeSpeed(newSpeed);
            yield return null;
        }

        if (_currentState == SpeedState.FinalSlowdown)
            _snake.ChangeSpeed(targetSpeed);

        _transitionCoroutine = null;
    }

    private IEnumerator SpeedTransitionCoroutine(float targetSpeed, float duration)
    {
        float startSpeed = _snake.MoveSpeed;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            _snake.ChangeSpeed(Mathf.Lerp(startSpeed, targetSpeed, t));
            yield return null;
        }

        _snake.ChangeSpeed(targetSpeed);
        _transitionCoroutine = null;
    }

    private enum SpeedState
    {
        Normal,
        Slowed,
        DeepSlowed,
        FinalSlowdown,
        Stopped
    }
}