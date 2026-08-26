using Game.Scripts.MapGenerator;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Road
{
    [RequireComponent(typeof(DirectionAnalyzer), typeof(RoadLimiter))]
    public class RoadSpawner : MonoBehaviour
    {
        private const int ROAD_GENERATE_MAX_ATTEMPTS_COUNT = 200;
        private const int SEGMENTS_GENERATE_MAX_ATTEMPTS_COUNT = 500;

        [SerializeField] private GameObject _stumpPrefab;
        [SerializeField] private BoundaryMaker _boundaryMaker;
        [SerializeField] private float _segmentLength = 2f;
        [SerializeField] private int _minPathSegments = 5;
        [SerializeField] private int _maxPathSegments = 15;

        [Header("SpawnRoutine Settings")]
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

        public List<Vector3> LastSpawnedRoad { get; } = new();

        private void Start()
        {
            _directionAnalyzer = GetComponent<DirectionAnalyzer>();
            _limiter = GetComponent<RoadLimiter>();
            CalculatePlayAreaLimits();
        }
        
        public bool TrySpawn(out List<Vector3> road)
        {
            road = null;
            LastSpawnedRoad.Clear();

            if (!GenerateValidRoad()) return false;
            
            road = LastSpawnedRoad;

            var spawnPosition = road[1];
            var lookPosition = road[2];

            if (!_stump)
                _stump = Instantiate(_stumpPrefab, spawnPosition, Quaternion.identity, transform);
            else
                _stump.transform.position = spawnPosition;

            _stump.transform.LookAt(lookPosition);
            return true;

        }
        
        private static Vector3[] GetAllPossibleDirections()
        {
            return new[]
            {
                Vector3.right,
                Vector3.left,
                Vector3.forward,
                Vector3.back
            };
        }
        
        private static bool LineSegmentsIntersect(Vector2 p1, Vector2 p2, Vector2 q1, Vector2 q2)
        {
            var s1_x = p2.x - p1.x;
            var s1_y = p2.y - p1.y;
            var s2_x = q2.x - q1.x;
            var s2_y = q2.y - q1.y;

            var s = (-s1_y * (p1.x - q1.x) + s1_x * (p1.y - q1.y)) / (-s2_x * s1_y + s1_x * s2_y);
            var t = (s2_x * (p1.y - q1.y) - s2_y * (p1.x - q1.x)) / (-s2_x * s1_y + s1_x * s2_y);

            return s is >= 0 and <= 1 && t is >= 0 and <= 1;
        }
        
        private static void DrawRectangle(float minX, float maxX, float minZ, float maxZ)
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

        private void CalculatePlayAreaLimits()
        {
            if (!_boundaryMaker)
            {
                SetDefaultLimits();
                return;
            }

            if (_boundaryMaker.TryGetBoundaryLimits(out var minX, out var maxX, out var minZ, out var maxZ))
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
        
        private bool GenerateRoad()
        {
            var currentDirection = _initialDirection;
            var currentPosition = _spawnPoint;
            var safetyCounter = 0;
            var startedFromTop = IsPointNearTopBoundary(_spawnPoint);

            while (LastSpawnedRoad.Count < _maxPathSegments && safetyCounter++ < SEGMENTS_GENERATE_MAX_ATTEMPTS_COUNT)
            {
                if (TryMoveForward(ref currentPosition, currentDirection))
                {
                    LastSpawnedRoad.Add(currentPosition);

                    if (IsOutsidePlayArea(currentPosition))
                    {
                        LastSpawnedRoad.RemoveAt(LastSpawnedRoad.Count - 1);
                        currentDirection = GetValidTurnDirection(currentDirection, 
                            currentPosition, true, startedFromTop);
                        
                        if (currentDirection == Vector3.zero) break;
                        continue;
                    }

                    if (ShouldTurn(LastSpawnedRoad.Count))
                    {
                        currentDirection = GetValidTurnDirection(currentDirection, 
                            currentPosition, false, startedFromTop);
                        
                        if (currentDirection == Vector3.zero) break;
                    }
                }
                else
                {
                    currentDirection = GetValidTurnDirection(currentDirection, 
                        currentPosition, false, startedFromTop);
                    
                    if (currentDirection == Vector3.zero) break;
                }

                if (_limiter.IsEndTooCloseToBoundary(currentPosition)) break;
            }

            return LastSpawnedRoad.Count >= _minPathSegments;
        }

        private bool GenerateValidRoad()
        {
            var attempts = 0;

            while (attempts++ < ROAD_GENERATE_MAX_ATTEMPTS_COUNT)
            {
                LastSpawnedRoad.Clear();
                InitializeStartingPointAndDirection();

                if (_spawnPoint == Vector3.zero) continue;

                if (!GenerateRoad() || LastSpawnedRoad.Count < _minPathSegments) continue;
                
                AddEntryPointBeforeStart();

                var lastPointValid = !_limiter.IsEndTooCloseToBoundary(LastSpawnedRoad[^1]);
                var noSelfIntersection = !HasSelfIntersection();
                var withinPlayArea = IsRoadWithinPlayArea();

                if (lastPointValid && noSelfIntersection && withinPlayArea) return true;
            }

            return false;
        }

        private void AddEntryPointBeforeStart()
        {
            if (LastSpawnedRoad.Count <= 0) return;
            var firstPoint = LastSpawnedRoad[0];
            var secondPoint = LastSpawnedRoad[1];

            var direction = (secondPoint - firstPoint).normalized;

            var entryPoint = firstPoint - direction * _segmentLength;

            LastSpawnedRoad.Insert(0, entryPoint);
        }

        private void InitializeStartingPointAndDirection()
        {
            var preferredSide = GetPreferredSpawnSide();
            _spawnPoint = _boundaryMaker.GetRandomPointOnSide(preferredSide);

            if (_spawnPoint == Vector3.zero) return;

            LastSpawnedRoad.Add(_spawnPoint);
            _initialDirection = GetInitialDirectionForSide(preferredSide);
        }

        private Vector3 GetInitialDirectionForSide(BoundaryMaker.BoundarySide side)
        {
            return side switch
            {
                BoundaryMaker.BoundarySide.Top => Vector3.back,
                BoundaryMaker.BoundarySide.Left => Vector3.right,
                BoundaryMaker.BoundarySide.Right => Vector3.left,
                BoundaryMaker.BoundarySide.Bottom => Vector3.forward,
                _ => _directionAnalyzer.GetValidDirection(_spawnPoint),
            };
        }

        private BoundaryMaker.BoundarySide GetPreferredSpawnSide()
        {
            List<BoundaryMaker.BoundarySide> availableSides = new();

            if (_allowTopSpawn) availableSides.Add(BoundaryMaker.BoundarySide.Top);
            if (_allowLeftSpawn) availableSides.Add(BoundaryMaker.BoundarySide.Left);
            if (_allowRightSpawn) availableSides.Add(BoundaryMaker.BoundarySide.Right);

            if (availableSides.Count == 0) return BoundaryMaker.BoundarySide.Top;

            var index = Random.Range(0, availableSides.Count);
            return availableSides[index];
        }

        private bool IsPointNearTopBoundary(Vector3 point)
        {
            return Mathf.Abs(point.z - _maxAllowedHeight) < 0.1f;
        }

        private bool TryMoveForward(ref Vector3 position, Vector3 direction)
        {
            var newPosition = position + direction * _segmentLength;

            if (!_limiter.IsPositionValid(newPosition, LastSpawnedRoad)) return false;
            position = newPosition;
            return true;

        }

        private bool ShouldTurn(int segmentCount)
        {
            var turnProbability = _initialTurnProbability + (segmentCount * _turnProbabilityIncrease);
            return Random.value < turnProbability;
        }

        private Vector3 GetValidTurnDirection(Vector3 currentDirection, 
            Vector3 currentPosition, bool avoidExtremeDirections, bool startedFromTop)
        {
            List<Vector3> validDirections = new();
            var possibleTurns = GetAllPossibleDirections();

            foreach (var turn in possibleTurns)
            {
                if (turn == -currentDirection) continue;

                var testPosition = currentPosition + turn * _segmentLength;

                var wouldBeOutside = IsOutsidePlayArea(testPosition);

                if (avoidExtremeDirections)
                {
                    if (startedFromTop && turn == Vector3.forward) continue;
                    if (turn == Vector3.forward && currentPosition.z >= _maxAllowedHeight - _segmentLength * 0.5f) continue;
                    if (turn == Vector3.back && currentPosition.z <= _minAllowedHeight + _segmentLength * 0.5f) continue;
                }

                if (_limiter.IsPositionValid(testPosition, LastSpawnedRoad) && !wouldBeOutside)
                {
                    validDirections.Add(turn);
                }
            }

            if (validDirections.Count != 0)
                return validDirections.Count > 0
                    ? validDirections[Random.Range(0, validDirections.Count)]
                    : Vector3.zero;
            {
                var testPosition = currentPosition + currentDirection * _segmentLength;
                if (_limiter.IsPositionValid(testPosition, LastSpawnedRoad) && !IsOutsidePlayArea(testPosition))
                {
                    return currentDirection;
                }
            }

            return validDirections.Count > 0 ? validDirections[Random.Range(0, validDirections.Count)] : Vector3.zero;
        }

        private bool IsOutsidePlayArea(Vector3 position)
        {
            return position.x < _minAllowedX || position.x > _maxAllowedX ||
                   position.z < _minAllowedHeight || position.z > _maxAllowedHeight;
        }

        private bool IsRoadWithinPlayArea()
        {
            for (var i = 1; i < LastSpawnedRoad.Count; i++)
            {
                if (IsOutsidePlayArea(LastSpawnedRoad[i])) return false;
            }

            return true;
        }

        private bool HasSelfIntersection()
        {
            for (var i = 0; i < LastSpawnedRoad.Count - 3; i++)
            {
                var p1 = LastSpawnedRoad[i];
                var p2 = LastSpawnedRoad[i + 1];

                for (var j = i + 2; j < LastSpawnedRoad.Count - 1; j++)
                {
                    var p3 = LastSpawnedRoad[j];
                    var p4 = LastSpawnedRoad[j + 1];

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
      
        private void OnDrawGizmos()
        {
            if (!Application.isPlaying) return;

            Gizmos.color = Color.cyan;
            DrawRectangle(_minAllowedX, _maxAllowedX, _minAllowedHeight, _maxAllowedHeight);

            if (LastSpawnedRoad.Count <= 0) return;
            
            Gizmos.color = Color.gray;
            if (LastSpawnedRoad.Count > 1)
            {
                Gizmos.DrawLine(LastSpawnedRoad[0], LastSpawnedRoad[1]);
                Gizmos.DrawSphere(LastSpawnedRoad[0], 0.15f);
            }

            Gizmos.color = Color.yellow;
            for (var i = 1; i < LastSpawnedRoad.Count - 1; i++)
            {
                Gizmos.DrawLine(LastSpawnedRoad[i], LastSpawnedRoad[i + 1]);
                Gizmos.DrawSphere(LastSpawnedRoad[i], 0.2f);
            }

            if (LastSpawnedRoad.Count > 1)
                Gizmos.DrawSphere(LastSpawnedRoad[^1], 0.2f);
        }
    }
}