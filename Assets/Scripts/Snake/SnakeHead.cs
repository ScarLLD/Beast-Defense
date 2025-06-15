using System;
using UnityEngine;

[RequireComponent(typeof(SnakeMover), typeof(SnakeRotator), typeof(SnakeTail))]
[RequireComponent(typeof(SnakeLocalSettings))]
public class SnakeHead : MonoBehaviour
{
    [SerializeField] private float _speed;

    private PathHolder _pathHolder;
    private SnakeMover _mover;
    private SnakeRotator _rotator;
    private SnakeTail _tail;
    private SnakeLocalSettings _localSettings;

    public float Speed => _speed;

    private void Awake()
    {
        _localSettings = GetComponent<SnakeLocalSettings>();
        _mover = GetComponent<SnakeMover>();
        _mover.Init(this);
        _rotator = GetComponent<SnakeRotator>();
        _rotator.Init(this);
        _tail = GetComponent<SnakeTail>();
    }

    private void OnEnable()
    {
        _mover.Arrived += ChangePosition;
    }

    private void OnDisable()
    {
        _mover.Arrived -= ChangePosition;
    }

    public void SetSpeed(float speed)
    {
        _speed = speed;
    }

    public void Init(PathHolder pathHolder, Transform snakeTransform)
    {
        _pathHolder = pathHolder;

        if (_pathHolder.TryGetStartPosition(out Vector3 startPosition))
        {
            _localSettings.SetTarget(startPosition);
            _mover.StartMoveRoutine();
            _rotator.SetStartRotation();
            _tail.Spawn(_localSettings.TargetPosition - startPosition, this, _pathHolder);
        }
    }

    private void ChangePosition()
    {
        if (_pathHolder.TryGetNextPosition(_localSettings.TargetPosition, out Vector3 nextPosition))
        {
            _localSettings.SetTarget(nextPosition);
            _rotator.StartRotateRoutine();
        }
        else
        {
            _mover.StopMoveRoutine();
            SetSpeed(0);
        }
    }
}
