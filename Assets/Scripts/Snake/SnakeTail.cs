using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SnakeHead))]
public class SnakeTail : MonoBehaviour
{
    private readonly float _lengthMultiplier = 0f;
    private readonly float _distanceBetweenSegments = 0.8f;
    private readonly float _distanceBetweenCubes = 0.5f;
    private readonly float _scaleMultiplier = 0.7f;
    private readonly float _sizeDivider = 2;
    private readonly float _DirectionMultiplier = 1f;
    private Coroutine _coroutine;

    private int _currentActiveSegmentsCount = 0;
    private TargetStorage _targetStorage;
    private Transform _lastSegment;
    private SnakeHead _snakeHead;
    private CubeStorage _cubeStorage;
    private Cube _cubePrefab;

    private ObjectPool<SnakeSegment> _pool;

    private void Awake()
    {
        _snakeHead = GetComponent<SnakeHead>();
    }

    public void Init(CubeStorage cubeStorage, Cube cubePrefab, SnakeSegment snakeSegmentPrefab, TargetStorage targetStorage)
    {
        _cubeStorage = cubeStorage;
        _cubePrefab = cubePrefab;
        _targetStorage = targetStorage;

        _pool = new ObjectPool<SnakeSegment>(snakeSegmentPrefab, this.transform.parent);
    }

    public void StartSpawn(Vector3 direction, List<SnakeSegment> segments)
    {
        _coroutine = StartCoroutine(Spawn(direction, segments));
    }

    private IEnumerator Spawn(Vector3 direction, List<SnakeSegment> segments)
    {
        bool isWork = true;
        Queue<ICube> stacks = new(_cubeStorage.Stacks);
        Vector3 centerPoint;

        while (isWork)
        {
            if (segments.Count - _targetStorage.Count < 10 && stacks.Count > 0)
            {
                var stack = stacks.Dequeue();
                int segmentCount = stack.Count / 4;

                for (int i = 0; i < segmentCount; i++)
                {
                    if (_lastSegment != null)
                        centerPoint = _lastSegment.position + -direction.normalized * 2 / _sizeDivider * _distanceBetweenSegments;
                    else
                        centerPoint = transform.position + _DirectionMultiplier * _distanceBetweenSegments * 2 * -direction.normalized;

                    var segment = _pool.GetObject();

                    segment.transform.SetPositionAndRotation(centerPoint, Quaternion.LookRotation(direction));

                    Vector3[] points = new Vector3[4];
                    Vector3 rightOffset = transform.right * _distanceBetweenCubes;
                    Vector3 upOffset = transform.up * _distanceBetweenCubes;

                    if (_lastSegment != null)
                    {
                        rightOffset = _lastSegment.right * _distanceBetweenCubes;
                        upOffset = _lastSegment.up * _distanceBetweenCubes;
                    }

                    points[0] = centerPoint + rightOffset + upOffset;
                    points[1] = centerPoint - rightOffset + upOffset;
                    points[2] = centerPoint - rightOffset - upOffset;
                    points[3] = centerPoint + rightOffset - upOffset;

                    if (segment.IsNullHead)
                    {
                        segment.Init(_snakeHead);

                        for (int l = 0; l < points.Length; l++)
                        {
                            Cube cube = Instantiate(_cubePrefab, points[l], Quaternion.identity, segment.transform);
                            cube.transform.localScale *= _scaleMultiplier;
                            cube.Init(stack.Material);
                            cube.GetSegment(segment);

                            segment.AddCube(cube);
                        }

                        segments.Add(segment);
                        segment.StartRoutine();
                    }

                    segment.SnakeMover.SetLengths((segment.transform.localScale.magnitude + _lengthMultiplier) / 2, segment.transform.localScale.magnitude / 2);
                    _snakeHead.SetPreviousSegments();

                    _lastSegment = segment.transform;
                }
            }

            yield return null;
        }
    }

    public float GetObjectSizeInLocalDirection(Vector3 localDirection)
    {
        if (!TryGetComponent<Collider>(out var collider)) return 0f;

        Vector3 worldDirection = transform.TransformDirection(localDirection.normalized);

        Bounds bounds = collider.bounds;
        float thickness = Vector3.Dot(bounds.size, worldDirection);

        return Mathf.Abs(thickness);
    }
}