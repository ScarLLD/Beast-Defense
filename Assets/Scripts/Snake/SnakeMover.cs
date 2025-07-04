using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SnakeSegment))]
public class SnakeMover : MonoBehaviour
{
    private Coroutine _coroutine;
    private SnakeHead _snakeHead;
    private readonly float _arrivalThreshold = 0.01f;

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
            if (IsForwardMoving == true)
            {
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, TargetPoint, _snakeHead.Speed * Time.deltaTime);

                if ((TargetPoint - transform.localPosition).magnitude < _arrivalThreshold)
                {
                    transform.localPosition = TargetPoint;
                    SelectPosition();
                }

                if (TargetPoint == Vector3.zero)
                    isWork = false;
            }

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


    public void SetPreviusPosition(Vector3 targetPostion)
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
        if (IsForwardMoving == true)
            SetNextPosition(TargetPoint);
        else
            SetPreviusPosition(TargetPoint);
    }
}
