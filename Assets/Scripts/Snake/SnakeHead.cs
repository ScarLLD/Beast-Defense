using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(SnakeSegment), typeof(SnakeTail), typeof(SnakeSpeedControl))]
public class SnakeHead : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private Cube _cubePrefab;
    [SerializeField] private SnakeSegment _snakeSegmentPrefab;

    private SnakeTail _tail;

    private List<SnakeSegment> _segments;
    private List<Vector3> _road;

    public int RoadCount => _road.Count;
    public float Speed => _speed;
    private SnakeSegment _snakeSegment;

    private void Awake()
    {
        _snakeSegment = GetComponent<SnakeSegment>();
        _tail = GetComponent<SnakeTail>();
    }

    public void ChangeSpeed(float speed)
    {
        _speed = speed;
    }

    public void Init(CubeStorage cubeStorage, List<Vector3> road)
    {
        _road = road;
        _tail.Init(cubeStorage, _cubePrefab, _snakeSegmentPrefab);
    }

    public bool TryGetFirstRoadPoint(out Vector3 point)
    {
        point = Vector3.zero;

        if (_road.Count > 0)
        {
            point = _road.First();
        }

        return point != Vector3.zero;
    }

    public bool TryGetLastRoadPoint(out Vector3 point)
    {
        point = Vector3.zero;

        if (_road.Count > 0)
        {
            point = _road.Last();
        }

        return point != Vector3.zero;
    }

    public void CreateTail()
    {
        if (TryGetFirstRoadPoint(out Vector3 firstPoint) && TryGetNextRoadPoint(firstPoint, out Vector3 secondPoint))
        {
            Vector3 direction = secondPoint - transform.position;

            _segments = new() { _snakeSegment };
            _snakeSegment.Init(this);
            _snakeSegment.SnakeMover.SetLengths(_snakeSegment.transform.localScale.magnitude / 2, _snakeSegment.transform.localScale.magnitude / 2);

            _tail.StartSpawn(direction);
        }
    }

    public bool TryGetNextRoadPoint(Vector3 point, out Vector3 nextPoint)
    {
        nextPoint = Vector3.zero;

        if (_road.Contains(point) == true && _road.Count > _road.IndexOf(point) + 1)
            nextPoint = _road[_road.IndexOf(point) + 1];
        else
            ChangeSpeed(0);

        return nextPoint != Vector3.zero;
    }

    public bool TryGetPreviousRoadPoint(Vector3 point, out Vector3 previusPoint)
    {
        previusPoint = Vector3.zero;

        if (_road.Contains(point) == true)
        {
            if (_road.IndexOf(point) - 1 > 0)
                previusPoint = _road[_road.IndexOf(point) - 1];
            else
                previusPoint = _road[0];
        }

        return previusPoint != Vector3.zero;
    }

    public void DeleteSegment(SnakeSegment snakeSegment)
    {
        if (_segments.Contains(snakeSegment))
        {
            _segments.Remove(snakeSegment);
            snakeSegment.gameObject.SetActive(false);
        }

        _tail.DecreaseActiveSegmentsCount();
        SetPreviousSegments();
    }

    public bool CompareRoadPoint(int minRoadCountToEnd, int thresholdSlowdown, out float duration)
    {
        duration = int.MaxValue;

        var point = _snakeSegment.SnakeMover.TargetPoint;

        if (_road.Contains(point) && minRoadCountToEnd >= _road.Count - _road.IndexOf(point))
        {
            duration = (_road.Last() - transform.position).magnitude;
        }

        return duration < thresholdSlowdown;
    }

    public void SetPreviousSegments()
    {
        for (int i = 0; i < _segments.Count - 1; i++)
        {
            _segments[i].SnakeMover.SetPreviousSegment(_segments[i + 1]);
        }
    }
}
