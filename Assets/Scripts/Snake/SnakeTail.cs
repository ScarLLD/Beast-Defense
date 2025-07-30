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

    private TargetStorage _targetStorage;
    private SnakeSegment _lastSegment;
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
        StartCoroutine(Spawn(direction, segments));
    }

    private IEnumerator Spawn(Vector3 direction, List<SnakeSegment> segments)
    {
        bool isWork = true;
        Queue<ICube> stacks = new(_cubeStorage.Stacks);
        Vector3 centerPoint;

        int totalCubes = 0;

        foreach (var stack in stacks)
        {
            totalCubes += stack.Count;
        }

        Debug.Log($"cubeStorage stacks {_cubeStorage.Stacks.Count}");
        Debug.Log($"Общее количество кубов в змее: {totalCubes}");

        while (isWork)
        {
            if (segments.Count - _targetStorage.Count < 10)
            {
                var stack = stacks.Dequeue();
                int segmentCount = stack.Count / 4;

                for (int i = 0; i < segmentCount; i++)
                {
                    if (_lastSegment != null)
                        centerPoint = _lastSegment.transform.position + -direction.normalized * 2 / _sizeDivider * _distanceBetweenSegments;
                    else
                        centerPoint = transform.position + _DirectionMultiplier * _distanceBetweenSegments * 2 * -direction.normalized;

                    var segment = _pool.GetObject();

                    segment.transform.SetPositionAndRotation(centerPoint, Quaternion.LookRotation(direction));

                    Vector3 rightOffset = transform.right * _distanceBetweenCubes;
                    Vector3 upOffset = transform.up * _distanceBetweenCubes;

                    if (_lastSegment != null)
                    {
                        rightOffset = _lastSegment.transform.right * _distanceBetweenCubes;
                        upOffset = _lastSegment.transform.up * _distanceBetweenCubes;
                    }

                    if (segment.IsNullHead)
                    {
                        segment.Init(_snakeHead);

                        Vector3[] points = new Vector3[4];

                        points[0] = centerPoint + rightOffset + upOffset;
                        points[1] = centerPoint - rightOffset + upOffset;
                        points[2] = centerPoint - rightOffset - upOffset;
                        points[3] = centerPoint + rightOffset - upOffset;

                        for (int j = 0; j < points.Length; j++)
                        {
                            Cube cube = Instantiate(_cubePrefab, points[j], Quaternion.identity, segment.transform);
                            cube.transform.localScale *= _scaleMultiplier;
                            cube.Init(stack.Material);
                            cube.GetSegment(segment);

                            segment.AddCube(cube);
                        }

                        segment.SnakeMover.SetLengths((segment.transform.localScale.magnitude + _lengthMultiplier) / 2, segment.transform.localScale.magnitude / 2);
                    }
                    else
                    {
                        segment.ActivateCubes(stack.Material);
                    }

                    segments.Add(segment);
                    segment.SnakeMover.SetPreviousSegment(_lastSegment);
                    segment.StartRoutine();

                    _lastSegment = segment;
                }

                Debug.Log(stacks.Count);
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