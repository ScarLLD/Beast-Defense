using System;
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
    private SnakeHead _snakeHead;
    private Queue<Cube> _cubes;

    public Material Material { get; private set; }

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

    public void Init(SnakeHead snakeHead, PathHolder pathHolder)
    {
        _pathHolder = pathHolder;
        _snakeHead = snakeHead;

        _mover.Init(snakeHead);
        _rotator.Init(snakeHead);

        Move();
    }

    public bool TryGetCube(out Cube cube)
    {
        cube = null;

        if (_cubes.Count > 0)
            cube = _cubes.Dequeue();

        return cube != null;
    }

    public void AddCube(Cube cube)
    {
        _cubes.Enqueue(cube);
        Material = cube.Material;
    }

    public void TryDestroy()
    {
        if (_cubes.Count == 0)
        {
            _snakeHead.ProcessLoss(this);
        }
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
