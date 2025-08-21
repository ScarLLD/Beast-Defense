using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SnakeSegment))]
public class SnakeMover : MonoBehaviour
{
    private readonly float _lengthMultiplier = 1.5f;
    private readonly float _arrivalThreshold = 0.05f;
    private float _thresholdBetweenSegments;
    private float _gapLengthBetweenSegments;
    private SnakeSegment _previousSegment;
    private SnakeHead _snakeHead;
    private Coroutine _coroutine;
    private bool isNewMover = true;

    public float SpeedMultiplier { get; private set; } = 4f;
    public bool IsForwardMoving { get; private set; } = true;
    public Vector3 TargetPoint { get; private set; }

    public event Action Arrival;

    public void InitLengths(float threshold, float gap)
    {
        _thresholdBetweenSegments = threshold;
        _gapLengthBetweenSegments = gap;
    }

    public void InitHead(SnakeHead snakeHead)
    {
        _snakeHead = snakeHead;
    }

    private void OnDisable()
    {
        StopMoveRoutine();
    }

    public void SetPreviousSegment(SnakeSegment previousSegment)
    {
        _previousSegment = previousSegment;
    }

    public void StartMoveRoutine()
    {
        _coroutine ??= StartCoroutine(MoveToTarget());
    }

    public void StopMoveRoutine()
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }
    }

    public bool TrySetNextPosition(Vector3 targetPostion)
    {
        if (_snakeHead.RoadCount > 0)
        {
            IsForwardMoving = true;

            if (targetPostion == Vector3.zero && _snakeHead.TryGetFirstRoadPoint(out Vector3 firstPoint))
            {
                TargetPoint = firstPoint;
                return true;
            }
            else if (_snakeHead.TryGetNextRoadPoint(TargetPoint, out Vector3 nextPoint))
            {
                TargetPoint = nextPoint;
                return true;
            }
        }

        return false;
    }

    public bool TrySetPreviousPosition()
    {
        if (_snakeHead.RoadCount > 0)
        {
            IsForwardMoving = false;

            if (_snakeHead.TryGetPreviousRoadPoint(TargetPoint, out Vector3 previusPoint))
            {
                TargetPoint = previusPoint;
                return true;
            }
        }

        return false;
    }

    private IEnumerator MoveToTarget()
    {
        bool isWork = true;
        float initialSpeed = _snakeHead.Speed;

        TrySetNextPosition(Vector3.zero);

        while (isWork == true)
        {
            float speed = _snakeHead.Speed;
            float thresholdBetweenSegments = _thresholdBetweenSegments;
            float gapLengthBetweenSegments = _gapLengthBetweenSegments;

            if (IsForwardMoving == false)
            {
                if (_snakeHead.Speed < initialSpeed)
                {
                    speed = initialSpeed;
                }

                speed *= SpeedMultiplier;
            }

            transform.localPosition = Vector3.MoveTowards(transform.localPosition, TargetPoint, speed * Time.deltaTime);

            if (_previousSegment != null)
            {
                if (transform.localRotation.eulerAngles.y - _previousSegment.transform.localRotation.eulerAngles.y > 20f)
                {
                    thresholdBetweenSegments *= _lengthMultiplier;
                    gapLengthBetweenSegments *= _lengthMultiplier;
                }

                float lengthBetweenSegments = (transform.localPosition - _previousSegment.transform.localPosition).magnitude;

                if (IsForwardMoving == true && (lengthBetweenSegments > thresholdBetweenSegments) && isNewMover == false)
                {
                    TrySetPreviousPosition();
                }
                else if (IsForwardMoving == false && (lengthBetweenSegments <= gapLengthBetweenSegments))
                {
                    TrySetNextPosition(TargetPoint);
                }
            }

            if ((TargetPoint - transform.localPosition).magnitude < _arrivalThreshold == true)
                Debug.Log("Дистанция приближения ниже указаной");


            if (TargetPoint != null && TargetPoint != Vector3.zero && (TargetPoint - transform.localPosition).magnitude < _arrivalThreshold)
            {
                Debug.Log("Точка достигнута.");

                transform.localPosition = TargetPoint;
                isNewMover = false;

                if (TrySelectPosition() == false)
                {
                    isWork = false;
                    Debug.Log("Позиция не выбрана.");
                }
                else
                    Debug.Log("Позиция выбрана.");
            }

            yield return null;
        }

        Arrival?.Invoke();
    }

    private bool TrySelectPosition()
    {
        if (_previousSegment != null && IsForwardMoving == false && TrySetPreviousPosition())
            return true;
        else if (TrySetNextPosition(TargetPoint))
            return true;

        return false;
    }
}