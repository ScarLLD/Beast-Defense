using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SnakeSegment))]
public class SnakeMover : MonoBehaviour
{
    // Ваши оригинальные переменные
    private float _lengthMultiplier = 1.5f;
    private readonly float _arrivalThreshold = 0.01f;
    private float _thresholdBetweenSegments;
    private float _gapLengthBetweenSegments;
    private SnakeSegment _previousSegment;
    private SnakeHead _snakeHead;
    private Coroutine _coroutine;
    private bool _isNewSegment = true;
    private bool _shouldFollowRoad = false; // Новый флаг

    public void SetLengths(float threshold, float gap)
    {
        _thresholdBetweenSegments = threshold;
        _gapLengthBetweenSegments = gap;
    }

    public void Init(SnakeHead snakeHead)
    {
        _snakeHead = snakeHead;
    }

    public float SpeedMultiplier { get; private set; } = 4f;
    public bool IsForwardMoving { get; private set; } = true;
    public Vector3 TargetPoint { get; private set; }

    public void SetPreviousSegment(SnakeSegment previousSegment)
    {
        _previousSegment = previousSegment;
        _shouldFollowRoad = false; // Сначала следуем за сегментом
    }

    public void StartMoveRoutine()
    {
        _coroutine = StartCoroutine(MoveToTarget());
    }

    private IEnumerator MoveToTarget()
    {
        bool isWork = true;
        float initialSpeed = _snakeHead.Speed;

        // Первая цель - позиция предыдущего сегмента
        if (_previousSegment != null)
        {
            TargetPoint = _previousSegment.transform.position;
        }

        while (isWork)
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

            // Если еще не достигли предыдущего сегмента
            if (!_shouldFollowRoad && _previousSegment != null)
            {
                TargetPoint = _previousSegment.transform.position;

                // Проверяем расстояние до предыдущего сегмента
                if (Vector3.Distance(transform.position, _previousSegment.transform.position) < _thresholdBetweenSegments)
                {
                    _shouldFollowRoad = true; // Теперь можно следовать по дороге
                    SetNextPosition(Vector3.zero); // Переключаемся на дорогу
                }
            }

            transform.localPosition = Vector3.MoveTowards(transform.localPosition, TargetPoint, speed * Time.deltaTime);

            if (_shouldFollowRoad && _previousSegment != null)
            {
                // Ваша оригинальная логика движения по дороге
                if (transform.localRotation.eulerAngles.y - _previousSegment.transform.localRotation.eulerAngles.y > 20f)
                {
                    thresholdBetweenSegments *= _lengthMultiplier;
                    gapLengthBetweenSegments *= _lengthMultiplier;
                }

                float lengthBetweenSegments = (transform.localPosition - _previousSegment.transform.localPosition).magnitude;

                if (IsForwardMoving && lengthBetweenSegments > thresholdBetweenSegments)
                {
                    SetPreviousPosition();
                }
                else if (!IsForwardMoving && lengthBetweenSegments <= gapLengthBetweenSegments)
                {
                    SetNextPosition(TargetPoint);
                }
            }

            if (_shouldFollowRoad && TargetPoint != Vector3.zero &&
                (TargetPoint - transform.localPosition).magnitude < _arrivalThreshold)
            {
                transform.localPosition = TargetPoint;
                _isNewSegment = false;
                SelectPosition();
            }

            yield return null;
        }
    }

    // Ваши оригинальные методы без изменений
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
        if (_previousSegment != null && !IsForwardMoving)
            SetPreviousPosition();
        else
            SetNextPosition(TargetPoint);
    }

    public void StopMoveRoutine()
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }
    }
}