using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

[RequireComponent(typeof(Beast))]
public class BeastMover : MonoBehaviour
{
    private readonly float _speedMultiplier = 4f;
    private readonly float _arrivalThreshold = 0.005f;
    private readonly float _escapeThreshold = 0.1f;
    private Queue<float> _targetPercentages;
    private Coroutine _coroutine;
    private Snake _snake;
    private BeastRotator _beastRotator;
    private Beast _beast;
    private SplineContainer _splineContainer;
    private float _currentSplinePosition;
    private bool _isMovementCompleted = false;

    public Vector3 TargetPoint { get; private set; }
    public bool IsMoving { get; private set; } = false;
    public float NormalizedDistance { get; private set; }

    private void Awake()
    {
        _beast = GetComponent<Beast>();
        _beastRotator = GetComponent<BeastRotator>();
    }

    public void Init(Snake snake)
    {
        _snake = snake;
    }

    public void SetRoadTarget(SplineContainer splineContainer)
    {
        _splineContainer = splineContainer;

        _targetPercentages = new Queue<float>();
        _targetPercentages.Enqueue(0.5f);
        _targetPercentages.Enqueue(0.75f);
        _targetPercentages.Enqueue(1.0f);

        _currentSplinePosition = 0.5f;
        UpdatePositionFromSpline();
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
            // ���� ����������� ���� ��� ���� ��� ��������, �� ���������� ��������
            if ((CheckSnakeProximity() || IsMoving) && _targetPercentages.Count > 0 && _isMovementCompleted == false)
            {
                // ���� �� �������� � ���������� ����, ����� ��������� �����
                if (IsMoving == false || currentTargetPercentage == 0f)
                {
                    currentTargetPercentage = _targetPercentages.Dequeue();
                    IsMoving = true;
                }

                // ��������� � ������� ������� �� �������
                float moveDistance = _snake.MoveSpeed * _speedMultiplier * Time.deltaTime / splineLength;
                _currentSplinePosition = Mathf.MoveTowards(_currentSplinePosition, currentTargetPercentage, moveDistance);

                // ��������� ������� �� ������ �������
                UpdatePositionFromSpline();

                // ��������� ���������� ����
                if (Mathf.Abs(_currentSplinePosition - currentTargetPercentage) < _arrivalThreshold)
                {
                    _currentSplinePosition = currentTargetPercentage;
                    UpdatePositionFromSpline();

                    // �������� ������� ���� - ���������������
                    IsMoving = false;
                    currentTargetPercentage = 0f;

                    // ���� ��� ���� ��������� ����� - ��������� ��������
                    if (_targetPercentages.Count == 0)
                    {
                        _isMovementCompleted = true;
                        _beastRotator.SetFinalRotation(Vector3.down);
                        Debug.Log("is OVER BEAST");
                        isWork = false;
                    }
                }
            }

            yield return null;
        }

        // �����������, ��� IsMoving �������� ����� ���������� ��������
        IsMoving = false;
    }

    private void UpdatePositionFromSpline()
    {
        if (_splineContainer != null)
        {
            // �������� ������� � ����������� �� ������� (���������� float3)
            _splineContainer.Spline.Evaluate(_currentSplinePosition, out float3 position, out float3 tangent, out float3 up);

            // ������������ � Vector3
            Vector3 worldPosition = position;
            Vector3 worldTangent = tangent;

            // ������������� �������
            transform.position = worldPosition;

            // ��������� ������� ����� ��� ��������
            TargetPoint = worldPosition + worldTangent.normalized;

            // ���� ��������, ������� ������ �� ����������� �������
            if (IsMoving && worldTangent != Vector3.zero)
            {
                _beastRotator.SetLookDirection(worldTangent.normalized);
            }
        }
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
        _beastRotator.StopRotateRoutine();
    }

    public float GetNormalizedDistance()
    {
        return _currentSplinePosition;
    }

    public bool IsMovementCompleted()
    {
        return _isMovementCompleted;
    }

    private bool CheckSnakeProximity()
    {
        return _beast.GetNormalizedDistance() - _snake.NormalizedDistance < _escapeThreshold;
    }
}