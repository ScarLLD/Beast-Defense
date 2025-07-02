using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SnakeMover), typeof(SnakeRotator), typeof(SnakeTail))]
[RequireComponent(typeof(SnakeLocalSettings))]
public class SnakeHead : MonoBehaviour
{
    [SerializeField] private float _speed;

    [SerializeField] private Cube _cubePrefab;
    [SerializeField] private SnakeSegment _snakeSegmentPrefab;

    private SnakeMover _mover;
    private SnakeRotator _rotator;
    private SnakeTail _tail;
    private SnakeLocalSettings _localSettings;
    private List<Vector3> _roadPoints;

    public float Speed => _speed;

    private void Awake()
    {
        _localSettings = GetComponent<SnakeLocalSettings>();
        _mover = GetComponent<SnakeMover>();
        _rotator = GetComponent<SnakeRotator>();
        _tail = GetComponent<SnakeTail>();
    }

    //private void OnEnable()
    //{
    //    _mover.Arrived += SetNextPosition;
    //}

    //private void OnDisable()
    //{
    //    _mover.Arrived -= SetNextPosition;
    //}

    public void SetSpeed(float speed)
    {
        _speed = speed;
    }

    public bool TryGetNextPoint(Vector3 point, out Vector3 nextPoint)
    {
        

        if (_roadPoints.Contains(point))
            index = _roadPoints.IndexOf(point);

        return index != null;
    }

    public bool TryGetPreviusPoint(Vector3 point, out Vector3 previusPoint) { }

    public void Init(List<Vector3> road, Transform snakeTransform, CubeStorage cubeStorage)
    {
        _roadPoints = road;

        _mover.Init(this);
        _rotator.Init(this);

        _tail.Init(cubeStorage, _cubePrefab, _snakeSegmentPrefab);

        _mover.SetNextPosition();

        if (_pathHolder.TryGetStartPosition(out Vector3 startPosition))
        {
            _localSettings.SetTarget(startPosition);
            _mover.StartMoveRoutine();
            _rotator.SetStartRotation();
            _tail.Spawn(_localSettings.TargetPosition - startPosition, this, _pathHolder);
        }
    }

    public void ProcessLoss(SnakeSegment snakeSegment)
    {
        _tail.ProcessLoss(snakeSegment);
    }


}
