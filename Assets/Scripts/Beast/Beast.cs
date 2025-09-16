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
    private Snake _snake;

    public Vector3 TargetPoint { get; private set; }
    public bool IsMoving { get; private set; } = false;

    public void Init(Snake snake, SplineContainer splineContainer)
    {
        if (snake == null)
            throw new ArgumentException("Snake не может быть null.", nameof(snake));

        if (splineContainer == null)
            throw new ArgumentNullException("splineContainer не может быть null.", nameof(splineContainer));

        _snake = snake;
        _splineContainer = splineContainer;
        _yOffset = transform.localScale.y / 2;

        SetRoadTarget(splineContainer);
        StartMoveRoutine();
    }

    public void SetRoadTarget(SplineContainer splineContainer)
    {
        _splineContainer = splineContainer;

        _targetPercentages = new Queue<float>();
        _targetPercentages.Enqueue(0.75f);
        _targetPercentages.Enqueue(1.0f);

        ApplyOffsetPosition();
    }

    public void StartMoveRoutine()
    {
        _coroutine ??= StartCoroutine(MoveRoutine());
    }

    private IEnumerator MoveRoutine()
    {
        bool isWork = true;
        float currentTargetPercentage = 0f;
        float splineLength = _splineContainer.Spline.GetLength();

        while (isWork)
        {
            if ((IsSnakeClose() || IsMoving) && _isMovementCompleted == false)
            {
                if (IsMoving == false || currentTargetPercentage == 0f)
                {
                    currentTargetPercentage = _targetPercentages.Dequeue();
                    IsMoving = true;
                }

                float moveDistance = _snake.MoveSpeed * _speedMultiplier * Time.deltaTime / splineLength;
                _currentSplinePosition = Mathf.MoveTowards(_currentSplinePosition, currentTargetPercentage, moveDistance);

                ApplyOffsetPosition();

                if (Mathf.Abs(_currentSplinePosition - currentTargetPercentage) < _arrivalThreshold)
                {
                    _currentSplinePosition = currentTargetPercentage;
                    ApplyOffsetPosition();

                    IsMoving = false;
                    currentTargetPercentage = 0f;

                    if (!_isRotating)
                    {
                        yield return StartCoroutine(RotateToFace());
                    }

                    if (_targetPercentages.Count == 0)
                    {
                        _isMovementCompleted = true;
                        isWork = false;
                    }
                }

            }

            yield return null;
        }

        IsMoving = false;
    }

    private void ApplyOffsetPosition()
    {
        if (_splineContainer != null)
        {
            _splineContainer.Spline.Evaluate(_currentSplinePosition, out float3 position, out float3 tangent, out float3 up);

            Vector3 offsetPosition = new(position.x, position.y + _yOffset, position.z);
            transform.SetPositionAndRotation(offsetPosition, Quaternion.LookRotation(tangent, up));
        }
    }

    private IEnumerator RotateToFace()
    {
        _isRotating = true;

        Quaternion targetRotation = Quaternion.LookRotation(Vector3.back);

        float timer = 0f;
        Quaternion startRotation = transform.rotation;

        while (timer < _rotateDuration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / _rotateDuration);
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);
            yield return null;
        }

        transform.rotation = targetRotation;
        _isRotating = false;
    }

    private void StopMoveRoutine()
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }

        IsMoving = false;
        _isMovementCompleted = true;
        StopRotateRoutine();
    }

    private bool IsSnakeClose()
    {
        return _currentSplinePosition - _snake.NormalizedDistance < _escapeThreshold;
    }

    public void StopRotateRoutine()
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }
        _isRotating = false;
    }
}