using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(RoadVizualizer), typeof(DirectionAnalyzer))]
public class RoadSpawner : MonoBehaviour
{
    [SerializeField] private SnakeSpawner _snakeSpawner;
    [SerializeField] private RoadVizualizer _roadVizualizer;
    [SerializeField] private BoundaryMaker _boundaryMaker;
    [SerializeField] private TargetDetector _detector;
    [SerializeField] private float _segmentLength = 2f;
    [SerializeField] private int _minPathSegments = 5;
    [SerializeField] private int _maxPathSegments = 15;

    private DirectionAnalyzer _directionAnalyzer;
    private RoadLimiter _limiter;

    private Vector3 _spawnPoint;
    private Vector3 _initialDirection;
    private readonly List<Vector3> _road = new();

    private void Awake()
    {
        _directionAnalyzer = GetComponent<DirectionAnalyzer>();
        _limiter = GetComponent<RoadLimiter>();
    }

    public void Spawn()
    {
        if (GenerateValidRoad())
        {
            _detector.transform.position = _road[1];
            _detector.EnableTrigger();
            _roadVizualizer.VisualizeRoad(_road);
            _snakeSpawner.Spawn(_road);
        }
        else
        {
            Debug.LogWarning("Не удалось сгенерировать дорогу.");
        }
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
                bool hasUpwardMovement = false;
                Vector3 lastPoint = _road[_road.Count - 1];

                for (int i = 1; i < _road.Count; i++)
                {
                    if (_road[i].z > _road[i - 1].z)
                    {
                        hasUpwardMovement = true;
                    }
                }

                bool lastPointValid = _limiter.IsEndTooCloseToBoundary(lastPoint) == false;

                if (hasUpwardMovement == false && lastPointValid == true)
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
        return UnityEngine.Random.value < turnProbability;
    }

    private Vector3 GetValidTurnDirection(Vector3 currentDirection, Vector3 currentPosition)
    {
        List<Vector3> validDirections = new List<Vector3>();
        Vector3[] possibleTurns = GetPossibleTurns(currentDirection);

        foreach (var turn in possibleTurns)
        {
            Vector3 testPosition = currentPosition + turn * _segmentLength;
            if (_limiter.IsPositionValid(testPosition, _road) && IsUpwardDirection(turn, currentDirection) == false)
            {
                validDirections.Add(turn);
            }
        }

        if (validDirections.Count == 0)
        {
            Vector3 testPosition = currentPosition + currentDirection * _segmentLength;
            if (_limiter.IsPositionValid(testPosition, _road))
            {
                return currentDirection;
            }
        }

        return validDirections.Count > 0 ? validDirections[UnityEngine.Random.Range(0, validDirections.Count)] : Vector3.zero;
    }

    private bool IsUpwardDirection(Vector3 direction, Vector3 currentDirection)
    {
        return direction.z > 0 && direction != currentDirection;
    }

    private Vector3[] GetPossibleTurns(Vector3 currentDirection)
    {
        if (currentDirection == Vector3.right || currentDirection == Vector3.left)
            return new Vector3[] { Vector3.forward, Vector3.back };
        else
            return new Vector3[] { Vector3.right, Vector3.left };
    }
}