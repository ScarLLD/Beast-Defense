using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(DirectionAnalyzer))]
public class RoadSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _stumpPrefab;
    [SerializeField] private BoundaryMaker _boundaryMaker;
    [SerializeField] private float _segmentLength = 2f;
    [SerializeField] private int _minPathSegments = 5;
    [SerializeField] private int _maxPathSegments = 15;
    
    [Header("Height Limits")]
    [SerializeField] private float _minHeightPercentage = 0.565f;
    [SerializeField] private float _maxHeightPercentage = 0.85f;

    private DirectionAnalyzer _directionAnalyzer;
    private RoadLimiter _limiter;
    private Camera _mainCamera;
    private float _minAllowedHeight;
    private float _maxAllowedHeight;

    private GameObject _stump;
    private Vector3 _spawnPoint;
    private Vector3 _initialDirection;
    private readonly List<Vector3> _road = new();

    public List<Vector3> LastSpawnedRoad => _road;

    private void Awake()
    {
        _directionAnalyzer = GetComponent<DirectionAnalyzer>();
        _limiter = GetComponent<RoadLimiter>();
        _mainCamera = Camera.main;

        if (_mainCamera != null)
        {            
            CalculateHeightLimits();
        }
    }

    private void CalculateHeightLimits()
    {
        Vector3 screenBottomCenter = new(_mainCamera.pixelWidth * 0.5f,
                                               _mainCamera.pixelHeight * _minHeightPercentage,
                                               0f);

        Ray ray = _mainCamera.ScreenPointToRay(screenBottomCenter);
        if (Physics.Raycast(ray, out RaycastHit hitMin))
        {
            _minAllowedHeight = hitMin.point.z;
        }
        else
        {
            Vector3 worldPoint = _mainCamera.ViewportToWorldPoint(
                new Vector3(0.5f, _minHeightPercentage, _mainCamera.nearClipPlane + 10f));
            _minAllowedHeight = worldPoint.z;
        }

        Vector3 screenTopCenter = new(_mainCamera.pixelWidth * 0.5f,
                                             _mainCamera.pixelHeight * _maxHeightPercentage,
                                             0f);

        ray = _mainCamera.ScreenPointToRay(screenTopCenter);
        if (Physics.Raycast(ray, out RaycastHit hitMax))
        {
            _maxAllowedHeight = hitMax.point.z;
        }
        else
        {
            Vector3 worldPoint = _mainCamera.ViewportToWorldPoint(
                new Vector3(0.5f, _maxHeightPercentage, _mainCamera.nearClipPlane + 10f));
            _maxAllowedHeight = worldPoint.z;
        }
    }

    public bool TrySpawn(out List<Vector3> road)
    {
        road = null;
        _road.Clear();

        if (GenerateValidRoad())
        {
            road = _road;

            if (_stump == null)
                _stump = Instantiate(_stumpPrefab, road[0], Quaternion.identity, transform);
            else
                _stump.transform.position = road[0];

            _stump.transform.LookAt(road[1]);

            return true;
        }
        else
        {
            Debug.LogWarning("Не удалось сгенерировать дорогу.");
        }

        return false;
    }

    private bool GenerateRoad()
    {
        Vector3 currentDirection = _initialDirection;
        Vector3 currentPosition = _spawnPoint;
        int safetyCounter = 0;

        while (_road.Count < _maxPathSegments && safetyCounter++ < 200)
        {
            if (TryMoveForward(ref currentPosition, currentDirection))
            {
                _road.Add(currentPosition);

                if (IsOutsideHeightLimits(currentPosition))
                {
                    _road.RemoveAt(_road.Count - 1);
                    currentDirection = GetValidTurnDirection(currentDirection, currentPosition, true);
                    if (currentDirection == Vector3.zero) break;
                    continue;
                }

                if (ShouldTurn())
                {
                    currentDirection = GetValidTurnDirection(currentDirection, currentPosition);
                    if (currentDirection == Vector3.zero) break;
                }
            }
            else
            {
                currentDirection = GetValidTurnDirection(currentDirection, currentPosition);
                if (currentDirection == Vector3.zero) break;
            }

            if (_limiter.IsEndTooCloseToBoundary(currentPosition))
            {
                break;
            }
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

            if (GenerateRoad() && _road.Count >= _minPathSegments)
            {
                bool lastPointValid = _limiter.IsEndTooCloseToBoundary(_road[^1]) == false;
                bool noSelfIntersection = !HasSelfIntersection();
                bool withinHeightLimit = IsRoadWithinHeightLimit();

                if (lastPointValid && noSelfIntersection && withinHeightLimit)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void InitializeStartingPointAndDirection()
    {
        _spawnPoint = _boundaryMaker.GetRandomPointOnRandomLine();
        _road.Add(_spawnPoint);

        _initialDirection = _directionAnalyzer.GetValidDirection(_spawnPoint);
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

    private bool ShouldTurn()
    {
        float turnProbability = 0.3f + (_road.Count * 0.02f);
        return Random.value < turnProbability;
    }

    private Vector3 GetValidTurnDirection(Vector3 currentDirection, Vector3 currentPosition, bool avoidExtremeDirections = false)
    {
        List<Vector3> validDirections = new();
        Vector3[] possibleTurns = GetAllPossibleDirections();

        foreach (var turn in possibleTurns)
        {
            Vector3 testPosition = currentPosition + turn * _segmentLength;

            bool wouldBeOutsideLimits = IsOutsideHeightLimits(testPosition);

            if (avoidExtremeDirections)
            {
                if (turn == Vector3.forward && currentPosition.z >= _maxAllowedHeight * 0.9f)
                    continue;
                if (turn == Vector3.back && currentPosition.z <= _minAllowedHeight * 1.1f)
                    continue;
            }

            if (_limiter.IsPositionValid(testPosition, _road) && !wouldBeOutsideLimits)
            {
                validDirections.Add(turn);
            }
        }

        if (validDirections.Count == 0)
        {
            Vector3 testPosition = currentPosition + currentDirection * _segmentLength;
            if (_limiter.IsPositionValid(testPosition, _road) && !IsOutsideHeightLimits(testPosition))
            {
                return currentDirection;
            }
        }

        return validDirections.Count > 0 ? validDirections[Random.Range(0, validDirections.Count)] : Vector3.zero;
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

    private bool IsOutsideHeightLimits(Vector3 position)
    {
        return position.z < _minAllowedHeight || position.z > _maxAllowedHeight;
    }

    private bool IsRoadWithinHeightLimit()
    {
        foreach (var point in _road)
        {
            if (IsOutsideHeightLimits(point))
            {
                return false;
            }
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

                if (DoSegmentsIntersect(p1, p2, p3, p4))
                {
                    return true;
                }
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
        float s1_x, s1_y, s2_x, s2_y;
        s1_x = p2.x - p1.x; s1_y = p2.y - p1.y;
        s2_x = q2.x - q1.x; s2_y = q2.y - q1.y;

        float s, t;
        s = (-s1_y * (p1.x - q1.x) + s1_x * (p1.y - q1.y)) / (-s2_x * s1_y + s1_x * s2_y);
        t = (s2_x * (p1.y - q1.y) - s2_y * (p1.x - q1.x)) / (-s2_x * s1_y + s1_x * s2_y);

        return s >= 0 && s <= 1 && t >= 0 && t <= 1;
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying || _minAllowedHeight == 0 || _maxAllowedHeight == 0)
            return;

        if (_boundaryMaker.TryGetScreenWidthBounds(out float minX, out float maxX))
        {
            Gizmos.color = Color.red;
            Vector3 upperLeft = new(minX, 0, _maxAllowedHeight);
            Vector3 upperRight = new(maxX, 0, _maxAllowedHeight);
            Gizmos.DrawLine(upperLeft, upperRight);
            Gizmos.DrawSphere(upperLeft, 0.2f);
            Gizmos.DrawSphere(upperRight, 0.2f);

            Gizmos.color = Color.blue;
            Vector3 lowerLeft = new(minX, 0, _minAllowedHeight);
            Vector3 lowerRight = new(maxX, 0, _minAllowedHeight);
            Gizmos.DrawLine(lowerLeft, lowerRight);
            Gizmos.DrawSphere(lowerLeft, 0.2f);
            Gizmos.DrawSphere(lowerRight, 0.2f);

            Gizmos.color = new Color(0, 1, 0, 0.2f);
            for (float x = minX; x <= maxX; x += (maxX - minX) / 10f)
            {
                Gizmos.DrawLine(new Vector3(x, 0, _minAllowedHeight), 
                               new Vector3(x, 0, _maxAllowedHeight));
            }
        }
    }
}