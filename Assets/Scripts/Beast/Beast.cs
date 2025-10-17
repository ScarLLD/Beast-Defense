using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class Beast : MonoBehaviour
{
    [SerializeField] private float _speedMultiplier = 4f;
    [SerializeField] private float _rotationSpeed = 15f;
    [SerializeField] private float _rotateDuration = 0.3f;

    private readonly float _arrivalThreshold = 0.005f;
    private readonly float _escapeThreshold = 0.15f;
    private readonly float _startSplinePosition = 0.5f;
    private float _currentSplinePosition;
    private Vector3 _originalScale;
    private SplineContainer _splineContainer;
    private Queue<float> _targetPercentages;
    private Coroutine _rotateCoroutine;
    private Coroutine _moveCoroutine;
    private Transform _transform;
    private float _snakeSpeed;

    private float _cachedSplineLength;

    public bool IsMoving { get; private set; }

    private void Awake()
    {
        _transform = transform;
        _originalScale = _transform.localScale;
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

        _cachedSplineLength = _splineContainer.Spline.GetLength();

        SetDefaultSettings();
    }

    public void SetDefaultSettings()
    {
        Cleanup();

        IsMoving = false;

        gameObject.SetActive(true);
        _transform.localScale = _originalScale;

        _cachedSplineLength = _splineContainer.Spline.GetLength();

        _currentSplinePosition = _startSplinePosition;

        _targetPercentages.Enqueue(0.75f);
        _targetPercentages.Enqueue(1.0f);

        PlaceOnSpline();
        _rotateCoroutine = StartCoroutine(RotateToFace());
    }

    public bool TryApproachNotify(float snakeSplinePosition)
    {
        if (_currentSplinePosition - snakeSplinePosition < _escapeThreshold)
        {
            Debug.Log("entity close!");

            if (IsMoving == false && _targetPercentages.Count > 0)
            {
                if (_moveCoroutine != null)
                {
                    StopCoroutine(_moveCoroutine);
                    _moveCoroutine = null;
                }

                _moveCoroutine = StartCoroutine(MoveRoutine());
            }

            return true;
        }

        return false;
    }

    private void Cleanup()
    {
        if (_moveCoroutine != null)
        {
            StopCoroutine(_moveCoroutine);
            _moveCoroutine = null;

            IsMoving = false;
        }

        if (_rotateCoroutine != null)
        {
            StopCoroutine(_rotateCoroutine);
            _rotateCoroutine = null;
        }

        _targetPercentages.Clear();
    }

    private IEnumerator MoveRoutine()
    {
        float currentTargetPercentage = _targetPercentages.Dequeue();

        bool isWork = true;

        IsMoving = true;

        while (isWork)
        {
            float moveDistance = _snakeSpeed * _speedMultiplier * Time.deltaTime / _cachedSplineLength;
            _currentSplinePosition = Mathf.MoveTowards(_currentSplinePosition, currentTargetPercentage, moveDistance);

            PlaceOnSpline();

            if (Mathf.Abs(_currentSplinePosition - currentTargetPercentage) < _arrivalThreshold)
            {
                _currentSplinePosition = currentTargetPercentage;
                PlaceOnSpline();
                isWork = false;
            }

            yield return null;
        }

        yield return _rotateCoroutine = StartCoroutine(RotateToFace());
        IsMoving = false;
    }

    private void PlaceOnSpline()
    {
        if (_splineContainer != null)
        {
            _splineContainer.Spline.Evaluate(_currentSplinePosition, out float3 position, out float3 tangent, out float3 up);

            position.y += transform.localScale.y;

            _transform.position = position;

            if (IsMoving)
            {
                Vector3 safeTangent = (Vector3)tangent;
                Vector3 safeUp = (Vector3)up;

                if (safeTangent == Vector3.zero) safeTangent = Vector3.forward;
                if (safeUp == Vector3.zero) safeUp = Vector3.up;

                Quaternion targetRotation = Quaternion.LookRotation(safeTangent, safeUp);
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