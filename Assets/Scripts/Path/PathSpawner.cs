using UnityEngine;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(PathVizualizator), typeof(PathLimiter))]
[RequireComponent(typeof(PathHolder), typeof(DirectionHolder))]
public class PathSpawner : MonoBehaviour
{
    [SerializeField] private BoundaryMaker _boundaryMaker;
    [SerializeField] private float _segmentLength = 2f;
    [SerializeField] private int _minPathSegments = 5;
    [SerializeField] private int _maxPathSegments = 15;

    private DirectionHolder _directionHolder;
    private PathHolder _pathHolder;
    private PathVizualizator _vizualizator;
    private PathLimiter _limiter;

    private Vector3 _spawnPoint;
    private Vector3 _initialDirection;
    private List<Vector3> _pathPoints = new List<Vector3>();

    private void Awake()
    {
        _directionHolder = GetComponent<DirectionHolder>();
        _pathHolder = GetComponent<PathHolder>();
        _vizualizator = GetComponent<PathVizualizator>();
        _limiter = GetComponent<PathLimiter>();
    }

    public void SpawnPath()
    {
        if (GenerateValidPath())
        {
            _vizualizator.VisualizePath(_pathPoints);
            _pathHolder.InitPoints(_pathPoints);
        }
        else
        {
            Debug.LogWarning("Не удалось сгенерировать дорогу.");
        }
    }

    private bool GeneratePath()
    {
        Vector3 currentDirection = _initialDirection;        
        Vector3 currentPosition = _spawnPoint;
        int safetyCounter = 0;

        while (_pathPoints.Count < _maxPathSegments && safetyCounter++ < 200)
        {
            if (TryMoveForward(ref currentPosition, currentDirection))
            {
                _pathPoints.Add(currentPosition);

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

        return _pathPoints.Count >= _minPathSegments;
    }

    private bool GenerateValidPath()
    {
        int attempts = 0;
        int maxAttempts = 200;

        while (attempts++ < maxAttempts)
        {
            _pathPoints.Clear();
            InitializeStartingPointAndDirection();

            if (GeneratePath() && _pathPoints.Count >= _minPathSegments)
            {
                bool hasUpwardMovement = false;
                Vector3 lastPoint = _pathPoints[_pathPoints.Count - 1];

                for (int i = 1; i < _pathPoints.Count; i++)
                {
                    if (_pathPoints[i].z > _pathPoints[i - 1].z)
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
        _pathPoints.Add(_spawnPoint);

        _initialDirection = _directionHolder.GetValidDirection(_spawnPoint);
    }

    private bool TryMoveForward(ref Vector3 position, Vector3 direction)
    {
        Vector3 newPosition = position + direction * _segmentLength;
        if (_limiter.IsPositionValid(newPosition, _pathPoints))
        {
            position = newPosition;
            return true;
        }
        return false;
    }

    private bool ShouldTurn()
    {
        float turnProbability = 0.3f + (_pathPoints.Count * 0.02f);
        return UnityEngine.Random.value < turnProbability;
    }

    private Vector3 GetValidTurnDirection(Vector3 currentDirection, Vector3 currentPosition)
    {
        List<Vector3> validDirections = new List<Vector3>();
        Vector3[] possibleTurns = GetPossibleTurns(currentDirection);

        foreach (var turn in possibleTurns)
        {
            Vector3 testPosition = currentPosition + turn * _segmentLength;
            if (_limiter.IsPositionValid(testPosition, _pathPoints) && IsUpwardDirection(turn, currentDirection) == false)
            {
                validDirections.Add(turn);
            }
        }

        if (validDirections.Count == 0)
        {
            Vector3 testPosition = currentPosition + currentDirection * _segmentLength;
            if (_limiter.IsPositionValid(testPosition, _pathPoints))
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