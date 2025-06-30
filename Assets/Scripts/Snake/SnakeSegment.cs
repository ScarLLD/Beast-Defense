using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(SnakeMover), typeof(SnakeRotator), typeof(SnakeLocalSettings))]
public class SnakeSegment : MonoBehaviour
{
    private SnakeMover _mover;
    private SnakeRotator _rotator;
    private SnakeLocalSettings _localSettings;
    private PathHolder _pathHolder;

    private Queue<Cube> _cubes;

    private void Awake()
    {
        _mover = GetComponent<SnakeMover>();
        _rotator = GetComponent<SnakeRotator>();
        _localSettings = GetComponent<SnakeLocalSettings>();

        _cubes = new Queue<Cube>();
    }

    private void OnEnable()
    {
        _mover.Arrived += ChangePosition;
    }

    private void OnDisable()
    {
        _mover.Arrived -= ChangePosition;
    }

    public void AddCube(Cube cube)
    {
        _cubes.Enqueue(cube);
    }

    public void Init(SnakeHead snakeHead, PathHolder pathHolder)
    {
        _pathHolder = pathHolder;

        _mover.Init(snakeHead);
        _rotator.Init(snakeHead);

        Move();
    }

    public void Move()
    {
        if (_pathHolder.TryGetStartPosition(out Vector3 startPosition))
        {
            _localSettings.SetTarget(startPosition);
            _mover.StartMoveRoutine();
            _rotator.SetStartRotation();
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
        }
    }
}
