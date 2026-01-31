using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(DirectionAnalyzer), typeof(RoadLimiter))]
public class RoadSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _stumpPrefab;
    [SerializeField] private BoundaryMaker _boundaryMaker;
    [SerializeField] private float _segmentLength = 2f;
    [SerializeField] private int _minPathSegments = 5;
    [SerializeField] private int _maxPathSegments = 15;

    [Header("Spawn Settings")]
    [SerializeField] private bool _allowTopSpawn = true;
    [SerializeField] private bool _allowLeftSpawn = true;
    [SerializeField] private bool _allowRightSpawn = true;

    [Header("Pathfinding Settings")]
    [SerializeField] private float _initialTurnProbability = 0.3f;
    [SerializeField] private float _turnProbabilityIncrease = 0.02f;

    private DirectionAnalyzer _directionAnalyzer;
    private RoadLimiter _limiter;
    private float _minAllowedHeight;
    private float _maxAllowedHeight;
    private float _minAllowedX;
    private float _maxAllowedX;

    private GameObject _stump;
    private Vector3 _spawnPoint;
    private Vector3 _initialDirection;
    private readonly List<Vector3> _road = new();

    public List<Vector3> LastSpawnedRoad => _road;

    private void Start()
    {
        _directionAnalyzer = GetComponent<DirectionAnalyzer>();
        _limiter = GetComponent<RoadLimiter>();
        CalculatePlayAreaLimits();
    }

    private void CalculatePlayAreaLimits()
    {
        if (_boundaryMaker == null)
        {
            SetDefaultLimits();
            return;
        }

        if (_boundaryMaker.TryGetBoundaryLimits(out float minX, out float maxX, out float minZ, out float maxZ))
        {
            _minAllowedX = minX;
            _maxAllowedX = maxX;
            _minAllowedHeight = minZ;
            _maxAllowedHeight = maxZ;
        }
        else
        {
            SetDefaultLimits();
        }
    }

    private void SetDefaultLimits()
    {
        _minAllowedX = -8f;
        _maxAllowedX = 8f;
        _minAllowedHeight = -8f;
        _maxAllowedHeight = 8f;
    }

    public bool TrySpawn(out List<Vector3> road)
    {
        road = null;
        _road.Clear();

        if (GenerateValidRoad())
        {
            road = _road;

            Vector3 spawnPosition = road[1];
            Vector3 lookPosition = road[2];

            if (_stump == null)
                _stump = Instantiate(_stumpPrefab, spawnPosition, Quaternion.identity, transform);
            else
                _stump.transform.position = spawnPosition;

            _stump.transform.LookAt(lookPosition);
            return true;
        }

        return false;
    }

    private bool GenerateRoad()
    {
        Vector3 currentDirection = _initialDirection;
        Vector3 currentPosition = _spawnPoint;
        int safetyCounter = 0;
        bool startedFromTop = IsPointNearTopBoundary(_spawnPoint);

        while (_road.Count < _maxPathSegments && safetyCounter++ < 500)
        {
            if (TryMoveForward(ref currentPosition, currentDirection))
            {
                _road.Add(currentPosition);

                if (IsOutsidePlayArea(currentPosition))
                {
                    _road.RemoveAt(_road.Count - 1);
                    currentDirection = GetValidTurnDirection(currentDirection, currentPosition, true, startedFromTop);
                    if (currentDirection == Vector3.zero) break;
                    continue;
                }

                if (ShouldTurn(_road.Count))
                {
                    currentDirection = GetValidTurnDirection(currentDirection, currentPosition, false, startedFromTop);
                    if (currentDirection == Vector3.zero) break;
                }
            }
            else
            {
                currentDirection = GetValidTurnDirection(currentDirection, currentPosition, false, startedFromTop);
                if (currentDirection == Vector3.zero) break;
            }

            if (_limiter.IsEndTooCloseToBoundary(currentPosition)) break;
        }

        return _road.Count >= _minPathSegments;
    }

    private bool GenerateValidRoad()
    {
        int attempts = 0;
        int maxAttempts = 200;

        while (attempts++ < maxAttempts)
        {
            _road.Clear();
            InitializeStartingPointAndDirection();

            if (_spawnPoint == Vector3.zero) continue;

            if (GenerateRoad() && _road.Count >= _minPathSegments)
            {
                AddEntryPointBeforeStart();

                bool lastPointValid = !_limiter.IsEndTooCloseToBoundary(_road[^1]);
                bool noSelfIntersection = !HasSelfIntersection();
                bool withinPlayArea = IsRoadWithinPlayArea();

                if (lastPointValid && noSelfIntersection && withinPlayArea) return true;
            }
        }

        return false;
    }

    private void AddEntryPointBeforeStart()
    {
        if (_road.Count > 0)
        {
            Vector3 firstPoint = _road[0];
            Vector3 secondPoint = _road[1];

            Vector3 direction = (secondPoint - firstPoint).normalized;

            Vector3 entryPoint = firstPoint - direction * _segmentLength;

            _road.Insert(0, entryPoint);
        }
    }

    private void InitializeStartingPointAndDirection()
    {
        BoundaryMaker.BoundarySide preferredSide = GetPreferredSpawnSide();
        _spawnPoint = _boundaryMaker.GetRandomPointOnSide(preferredSide);

        if (_spawnPoint == Vector3.zero) return;

        _road.Add(_spawnPoint);
        _initialDirection = GetInitialDirectionForSide(preferredSide);
    }

    private Vector3 GetInitialDirectionForSide(BoundaryMaker.BoundarySide side)
    {
        switch (side)
        {
            case BoundaryMaker.BoundarySide.Top: return Vector3.back;
            case BoundaryMaker.BoundarySide.Left: return Vector3.right;
            case BoundaryMaker.BoundarySide.Right: return Vector3.left;
            case BoundaryMaker.BoundarySide.Bottom: return Vector3.forward;
            default: return _directionAnalyzer.GetValidDirection(_spawnPoint);
        }
    }

    private BoundaryMaker.BoundarySide GetPreferredSpawnSide()
    {
        List<BoundaryMaker.BoundarySide> availableSides = new();

        if (_allowTopSpawn) availableSides.Add(BoundaryMaker.BoundarySide.Top);
        if (_allowLeftSpawn) availableSides.Add(BoundaryMaker.BoundarySide.Left);
        if (_allowRightSpawn) availableSides.Add(BoundaryMaker.BoundarySide.Right);

        if (availableSides.Count == 0) return BoundaryMaker.BoundarySide.Top;

        int index = Random.Range(0, availableSides.Count);
        return availableSides[index];
    }

    private bool IsPointNearTopBoundary(Vector3 point)
    {
        return Mathf.Abs(point.z - _maxAllowedHeight) < 0.1f;
    }

    private bool TryMoveForward(ref Vector3 position, Vector3 direction)
    {
        Vector3 newPosition = position + direction * _segmentLength;
        if (_limiter.IsPositionValid(newPosition, _road))
        {
            position = newPosition;
            return true;
        }
        return false;
    }

    private bool ShouldTurn(int segmentCount)
    {
        float turnProbability = _initialTurnProbability + (segmentCount * _turnProbabilityIncrease);
        return Random.value < turnProbability;
    }

    private Vector3 GetValidTurnDirection(Vector3 currentDirection, Vector3 currentPosition, bool avoidExtremeDirections, bool startedFromTop)
    {
        List<Vector3> validDirections = new();
        Vector3[] possibleTurns = GetAllPossibleDirections();

        foreach (var turn in possibleTurns)
        {
            if (turn == -currentDirection) continue;

            Vector3 testPosition = currentPosition + turn * _segmentLength;

            bool wouldBeOutside = IsOutsidePlayArea(testPosition);

            if (avoidExtremeDirections)
            {
                if (startedFromTop && turn == Vector3.forward) continue;
                if (turn == Vector3.forward && currentPosition.z >= _maxAllowedHeight - _segmentLength * 0.5f) continue;
                if (turn == Vector3.back && currentPosition.z <= _minAllowedHeight + _segmentLength * 0.5f) continue;
            }

            if (_limiter.IsPositionValid(testPosition, _road) && !wouldBeOutside)
            {
                validDirections.Add(turn);
            }
        }

        if (validDirections.Count == 0)
        {
            Vector3 testPosition = currentPosition + currentDirection * _segmentLength;
            if (_limiter.IsPositionValid(testPosition, _road) && !IsOutsidePlayArea(testPosition))
            {
                return currentDirection;
            }
        }

        return validDirections.Count > 0 ? validDirections[UnityEngine.Random.Range(0, validDirections.Count)] : Vector3.zero;
    }

    private Vector3[] GetAllPossibleDirections()
    {
        return new Vector3[]
        {
            Vector3.right,
            Vector3.left,
            Vector3.forward,
            Vector3.back
        };
    }

    private bool IsOutsidePlayArea(Vector3 position)
    {
        return position.x < _minAllowedX || position.x > _maxAllowedX ||
               position.z < _minAllowedHeight || position.z > _maxAllowedHeight;
    }

    private bool IsRoadWithinPlayArea()
    {
        for (int i = 1; i < _road.Count; i++)
        {
            if (IsOutsidePlayArea(_road[i])) return false;
        }
        return true;
    }

    private bool HasSelfIntersection()
    {
        for (int i = 0; i < _road.Count - 3; i++)
        {
            Vector3 p1 = _road[i];
            Vector3 p2 = _road[i + 1];

            for (int j = i + 2; j < _road.Count - 1; j++)
            {
                Vector3 p3 = _road[j];
                Vector3 p4 = _road[j + 1];

                if (DoSegmentsIntersect(p1, p2, p3, p4)) return true;
            }
        }
        return false;
    }

    private bool DoSegmentsIntersect(Vector3 a1, Vector3 a2, Vector3 b1, Vector3 b2)
    {
        Vector2 a1_2d = new(a1.x, a1.z);
        Vector2 a2_2d = new(a2.x, a2.z);
        Vector2 b1_2d = new(b1.x, b1.z);
        Vector2 b2_2d = new(b2.x, b2.z);

        return LineSegmentsIntersect(a1_2d, a2_2d, b1_2d, b2_2d);
    }

    private bool LineSegmentsIntersect(Vector2 p1, Vector2 p2, Vector2 q1, Vector2 q2)
    {
        float s1_x = p2.x - p1.x;
        float s1_y = p2.y - p1.y;
        float s2_x = q2.x - q1.x;
        float s2_y = q2.y - q1.y;

        float s = (-s1_y * (p1.x - q1.x) + s1_x * (p1.y - q1.y)) / (-s2_x * s1_y + s1_x * s2_y);
        float t = (s2_x * (p1.y - q1.y) - s2_y * (p1.x - q1.x)) / (-s2_x * s1_y + s1_x * s2_y);

        return s >= 0 && s <= 1 && t >= 0 && t <= 1;
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        Gizmos.color = Color.cyan;
        DrawRectangle(_minAllowedX, _maxAllowedX, _minAllowedHeight, _maxAllowedHeight);

        if (_road.Count > 0)
        {
            Gizmos.color = Color.gray;
            if (_road.Count > 1)
            {
                Gizmos.DrawLine(_road[0], _road[1]);
                Gizmos.DrawSphere(_road[0], 0.15f);
            }

            Gizmos.color = Color.yellow;
            for (int i = 1; i < _road.Count - 1; i++)
            {
                Gizmos.DrawLine(_road[i], _road[i + 1]);
                Gizmos.DrawSphere(_road[i], 0.2f);
            }

            if (_road.Count > 1)
                Gizmos.DrawSphere(_road[^1], 0.2f);
        }
    }

    private void DrawRectangle(float minX, float maxX, float minZ, float maxZ)
    {
        Vector3 tl = new(minX, 0, maxZ);
        Vector3 tr = new(maxX, 0, maxZ);
        Vector3 bl = new(minX, 0, minZ);
        Vector3 br = new(maxX, 0, minZ);

        Gizmos.DrawLine(tl, tr);
        Gizmos.DrawLine(tr, br);
        Gizmos.DrawLine(br, bl);
        Gizmos.DrawLine(bl, tl);
    }
}