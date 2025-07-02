using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class SnakeMover : MonoBehaviour
{
    private Coroutine _coroutine;
    private SnakeHead _snakeHead;
    private readonly float _arrivalThreshold = 0.01f;

    public event Action Arrived;

    public void Init(SnakeHead snakeHead)
    {
        _snakeHead = snakeHead;
    }

    public bool IsForwardMoving;
    public Vector3 TargetPosition { get; private set; }

    private void Start()
    {
        StartMoveRoutine();
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

    public void SetNextPosition()
    {
        if (_snakeHead._roadPoints.Count > 0)
        {
            IsForwardMoving = true;

            if (TargetPosition == null)
            {
                TargetPosition = _snakeHead._roadPoints.First();
            }
            else if (_snakeHead._roadPoints.Contains(TargetPosition))
            {
                int index = _snakeHead._roadPoints.IndexOf(TargetPosition);

                if (_snakeHead.RoadPointsCount > index + 1)
                    TargetPosition = _snakeHead._roadPoints[index + 1];
            }
        }
    }

    public void SetPreviusPosition()
    {
        if (_snakeHead.RoadPointsCount > 0)
        {
            IsForwardMoving = false;

            if (_snakeHead.TryGetNextPoint(TargetPosition, out int index))
            {
                if (index > 0)
                {
                    TargetPosition = _snakeHead._roadPoints[index - 1];

                }
            }
        }
    }

    private IEnumerator MoveToTarget()
    {
        while (TargetPosition != null)
        {
            if (IsForwardMoving == true)
            {
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, TargetPosition, _snakeHead.Speed * Time.deltaTime);

                if ((TargetPosition - transform.localPosition).magnitude < _arrivalThreshold)
                {
                    transform.localPosition = TargetPosition;
                    Arrived?.Invoke();
                }
            }

            yield return null;
        }
    }
}
