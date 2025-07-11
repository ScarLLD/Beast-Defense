using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SnakeSegment))]
public class SnakeMover : MonoBehaviour
{
    private float _lengthMultiplier = 1.5f;
    private readonly float _arrivalThreshold = 0.01f;
    private float _thresholdBetweenSegments;
    private float _gapLengthBetweenSegments;
    private SnakeSegment _previousSegment;
    private SnakeHead _snakeHead;
    private Coroutine _coroutine;

    public float SpeedMultiplier { get; private set; } = 4f;

    public void SetLengths(float threshold, float gap)
    {
        _thresholdBetweenSegments = threshold;
        _gapLengthBetweenSegments = gap;

    }

    public void Init(SnakeHead snakeHead)
    {
        _snakeHead = snakeHead;
    }

    public bool IsForwardMoving { get; private set; } = true;
    public Vector3 TargetPoint { get; private set; }

    public void SetPreviousSegment(SnakeSegment previousSegment)
    {
        _previousSegment = previousSegment;
    }

    public void StartMoveRoutine()
    {
        _coroutine = StartCoroutine(MoveToTarget());
    }

    public void StopMoveRoutine()
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }
    }

    private IEnumerator MoveToTarget()
    {
        bool isWork = true;
        float initialSpeed = _snakeHead.Speed;

        SetNextPosition(Vector3.zero);

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

                if (IsForwardMoving == true && (lengthBetweenSegments > thresholdBetweenSegments))
                {
                    SetPreviousPosition();
                }
                else if (IsForwardMoving == false && (lengthBetweenSegments <= gapLengthBetweenSegments))
                {
                    SetNextPosition(TargetPoint);
                }
            }

            if ((TargetPoint - transform.localPosition).magnitude < _arrivalThreshold)
            {
                transform.localPosition = TargetPoint;
                SelectPosition();
            }

            if (TargetPoint == Vector3.zero)
                isWork = false;

            yield return null;
        }
    }

    public void SetNextPosition(Vector3 targetPostion)
    {
        if (_snakeHead.RoadCount > 0)
        {
            IsForwardMoving = true;

            if (targetPostion == Vector3.zero && _snakeHead.TryGetFirstRoadPoint(out Vector3 firstPoint))
            {
                TargetPoint = firstPoint;
            }
            else if (_snakeHead.TryGetNextRoadPoint(TargetPoint, out Vector3 nextPoint))
            {
                TargetPoint = nextPoint;
            }
        }
    }

    public void SetPreviousPosition()
    {
        if (_snakeHead.RoadCount > 0)
        {
            IsForwardMoving = false;

            if (_snakeHead.TryGetPreviousRoadPoint(TargetPoint, out Vector3 previusPoint))
            {
                TargetPoint = previusPoint;
            }
        }
    }

    private void SelectPosition()
    {
        if (_previousSegment != null && IsForwardMoving == false)
            SetPreviousPosition();
        else
            SetNextPosition(TargetPoint);
    }
}
