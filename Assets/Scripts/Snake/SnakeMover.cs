using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SnakeSegment))]
public class SnakeMover : MonoBehaviour
{
    private float _speedMultiplier = 1.2f;
    private readonly float _arrivalThreshold = 0.01f;
    private SnakeSegment _targetSegment;
    private Coroutine _coroutine;
    private SnakeHead _snakeHead;

    public void Init(SnakeHead snakeHead)
    {
        _snakeHead = snakeHead;
    }

    public bool IsForwardMoving { get; private set; } = true;
    public Vector3 TargetPoint { get; private set; }

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

        SetNextPosition(Vector3.zero);

        while (isWork == true)
        {
            float speed = _snakeHead.Speed;

            if (IsForwardMoving == false)
                speed *= _speedMultiplier;

            transform.localPosition = Vector3.MoveTowards(transform.localPosition, TargetPoint, speed * Time.deltaTime);

            if (_targetSegment != null && (_targetSegment.transform.position - transform.localPosition).magnitude < _arrivalThreshold)
            {
                _targetSegment = null;
            }

            if ((TargetPoint - transform.localPosition).magnitude < _arrivalThreshold)
            {
                transform.localPosition = TargetPoint;

                if (_targetSegment == null)
                    SelectPosition();
                else
                    SetNextPosition(TargetPoint);

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


    public void SetPreviusPosition()
    {
        if (_snakeHead.RoadCount > 0)
        {
            IsForwardMoving = false;

            if (_snakeHead.TryGetPreviusRoadPoint(TargetPoint, out Vector3 previusPoint))
            {
                TargetPoint = previusPoint;
            }
        }
    }

    private void SelectPosition()
    {
        if (_targetSegment != null || IsForwardMoving == false)
            SetPreviusPosition();
        else
            SetNextPosition(TargetPoint);
    }
}
