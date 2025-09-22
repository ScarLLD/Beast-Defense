using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class Beast : MonoBehaviour
{
    [SerializeField] private ParticleSystem _dieParticle;
    [SerializeField] private float _speedMultiplier = 4f;
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private float _rotateDuration = 0.3f;

    private readonly float _arrivalThreshold = 0.005f;
    private readonly float _escapeThreshold = 0.15f;
    private float _currentSplinePosition = 0.5f;
    private bool _isMovementCompleted = false;
    private bool _isRotating = false;
    private float _yOffset;
    private SplineContainer _splineContainer;
    private Queue<float> _targetPercentages;
    private Coroutine _coroutine;
    private Transform _transform;
    private Snake _snake;

    private float _cachedSplineLength;
    private bool _cachedSnakeClose;
    private float _lastSnakeCheckTime;
    private const float SNAKE_CHECK_INTERVAL = 0.1f;

    public bool IsMoving { get; private set; } = false;

    private void Awake()
    {
        _transform = transform;
    }

    public void Init(Snake snake, SplineContainer splineContainer)
    {
        if (snake == null)
            throw new ArgumentException("Snake не может быть null.", nameof(snake));

        if (splineContainer == null)
            throw new ArgumentNullException("splineContainer не может быть null.", nameof(splineContainer));

        _snake = snake;
        _splineContainer = splineContainer;
        _yOffset = _transform.localScale.y / 2;

        _cachedSplineLength = _splineContainer.Spline.GetLength();

        SetRoadTarget(splineContainer);
        StartMoveRoutine();
    }

    public void SetRoadTarget(SplineContainer splineContainer)
    {
        _splineContainer = splineContainer;
        _cachedSplineLength = _splineContainer.Spline.GetLength();

        _targetPercentages = new Queue<float>();
        _targetPercentages.Enqueue(0.75f);
        _targetPercentages.Enqueue(1.0f);

        ApplyOffsetPosition();
    }

    public void StartMoveRoutine()
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
        }
        _coroutine = StartCoroutine(MoveRoutineOptimized());
    }

    private IEnumerator MoveRoutineOptimized()
    {
        float currentTargetPercentage = 0f;
        bool hasTarget = false;

        while (!_isMovementCompleted)
        {
            UpdateSnakeCloseCache();

            if ((_cachedSnakeClose || IsMoving) && !_isMovementCompleted)
            {
                if (!hasTarget && _targetPercentages.Count > 0)
                {
                    currentTargetPercentage = _targetPercentages.Dequeue();
                    hasTarget = true;
                    IsMoving = true;
                }

                if (hasTarget)
                {
                    float moveDistance = _snake.MoveSpeed * _speedMultiplier * Time.deltaTime / _cachedSplineLength;
                    _currentSplinePosition = Mathf.MoveTowards(_currentSplinePosition, currentTargetPercentage, moveDistance);

                    ApplyOffsetPosition();

                    if (Mathf.Abs(_currentSplinePosition - currentTargetPercentage) < _arrivalThreshold)
                    {
                        _currentSplinePosition = currentTargetPercentage;
                        ApplyOffsetPosition();

                        IsMoving = false;
                        hasTarget = false;

                        if (!_isRotating)
                        {
                            yield return StartCoroutine(RotateToFace());
                        }

                        if (_targetPercentages.Count == 0)
                        {
                            _isMovementCompleted = true;
                            break;
                        }
                    }
                }
            }

            yield return null;
        }

        IsMoving = false;
    }

    private void UpdateSnakeCloseCache()
    {
        if (Time.time - _lastSnakeCheckTime > SNAKE_CHECK_INTERVAL)
        {
            _cachedSnakeClose = IsSnakeClose();
            _lastSnakeCheckTime = Time.time;
        }
    }

    private void ApplyOffsetPosition()
    {
        if (_splineContainer != null)
        { 
            _splineContainer.Spline.Evaluate(_currentSplinePosition, out float3 position, out float3 tangent, out float3 up);

            Vector3 offsetPosition = new Vector3(position.x, position.y + _yOffset, position.z);

            if (Vector3.Distance(_transform.position, offsetPosition) > 0.001f)
            {
                _transform.position = offsetPosition;
            }

            if (IsMoving)
            {
                Quaternion targetRotation = Quaternion.LookRotation(tangent, up);
                if (Quaternion.Angle(_transform.rotation, targetRotation) > 0.1f)
                {
                    _transform.rotation = Quaternion.Lerp(_transform.rotation, targetRotation,
                        _rotationSpeed * Time.deltaTime);
                }
            }
        }
    }

    private IEnumerator RotateToFace()
    {
        _isRotating = true;

        Quaternion targetRotation = Quaternion.LookRotation(Vector3.back);
        Quaternion startRotation = _transform.rotation;

        float timer = 0f;
        float inverseDuration = 1f / _rotateDuration;

        while (timer < _rotateDuration)
        {
            timer += Time.deltaTime;
            float t = timer * inverseDuration;
            _transform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);
            yield return null;
        }

        _transform.rotation = targetRotation;
        _isRotating = false;
    }

    private bool IsSnakeClose()
    {
        return _currentSplinePosition - _snake.NormalizedDistance < _escapeThreshold;
    }

    private void OnDestroy()
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
        }
    }
}