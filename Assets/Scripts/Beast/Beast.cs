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
    private readonly float _escapeThreshold = 0.3f;
    private float _startSplinePosition = 0.5f;
    private float _currentSplinePosition;
    private float _yOffset;
    private SplineContainer _splineContainer;
    private Queue<float> _targetPercentages;
    private Coroutine _rotateCoroutine;
    private Coroutine _moveCoroutine;
    private Transform _transform;
    private float _snakeSpeed;

    private float _cachedSplineLength;

    public bool IsMoving { get; private set; } = false;

    private void Awake()
    {
        _transform = transform;
        _targetPercentages = new Queue<float>();
    }

    public void Init(float snakeSpeed, SplineContainer splineContainer)
    {
        if (snakeSpeed < 0)
            throw new ArgumentException("SnakeSpeed не может быть меньше 0.", nameof(snakeSpeed));

        if (splineContainer == null)
            throw new ArgumentNullException("splineContainer не может быть null.", nameof(splineContainer));

        _snakeSpeed = snakeSpeed;
        _splineContainer = splineContainer;
        _yOffset = _transform.localScale.y / 2;

        _cachedSplineLength = _splineContainer.Spline.GetLength();

        SetDefaultSettings(splineContainer);
    }

    public void SetDefaultSettings(SplineContainer splineContainer)
    {
        Cleanup();

        _splineContainer = splineContainer;
        _cachedSplineLength = _splineContainer.Spline.GetLength();

        _currentSplinePosition = _startSplinePosition;

        _targetPercentages.Enqueue(0.75f);
        _targetPercentages.Enqueue(1.0f);

        ApplyOffsetPosition();
        _rotateCoroutine = StartCoroutine(RotateToFace());
    }

    public void ApproachNotify(float normalizedDistance)
    {
        if (IsMoving == false && _targetPercentages.Count > 0 && _currentSplinePosition - normalizedDistance < _escapeThreshold)
        {
            IsMoving = true;

            if (_moveCoroutine != null)
            {
                StopCoroutine(_moveCoroutine);
                _moveCoroutine = null;
            }

            _moveCoroutine = StartCoroutine(MoveRoutine());
        }
    }

    private void Cleanup()
    {
        if (_moveCoroutine != null)
        {
            StopCoroutine(_moveCoroutine);
            _moveCoroutine = null;
        }

        if (_rotateCoroutine != null)
        {
            StopCoroutine(_rotateCoroutine);
            _rotateCoroutine = null;
        }
    }

    private IEnumerator MoveRoutine()
    {
        float currentTargetPercentage = _targetPercentages.Dequeue();
        bool isWork = true;

        while (isWork)
        {
            float moveDistance = _snakeSpeed * _speedMultiplier * Time.deltaTime / _cachedSplineLength;
            _currentSplinePosition = Mathf.MoveTowards(_currentSplinePosition, currentTargetPercentage, moveDistance);

            ApplyOffsetPosition();

            if (Mathf.Abs(_currentSplinePosition - currentTargetPercentage) < _arrivalThreshold)
            {
                _currentSplinePosition = currentTargetPercentage;
                ApplyOffsetPosition();
                isWork = false;
            }

            yield return null;
        }

        yield return _rotateCoroutine = StartCoroutine(RotateToFace());
        IsMoving = false;
    }

    private void ApplyOffsetPosition()
    {
        if (_splineContainer != null)
        {
            _splineContainer.Spline.Evaluate(_currentSplinePosition, out float3 position, out float3 tangent, out float3 up);

            Vector3 offsetPosition = new(position.x, position.y + _yOffset, position.z);

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
    }

    private void OnDestroy()
    {
        if (_moveCoroutine != null)
        {
            StopCoroutine(_moveCoroutine);
        }
    }
}