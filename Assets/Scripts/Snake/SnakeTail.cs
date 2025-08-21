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

    private void OnDisable()
    {
        EndSpawn();
    }

    public void Init(CubeStorage cubeStorage, Cube cubePrefab, SnakeSegment snakeSegmentPrefab, TargetStorage targetStorage)
    {
        _cubeStorage = cubeStorage;
        _cubePrefab = cubePrefab;
        _targetStorage = targetStorage;

        _pool = new ObjectPool<SnakeSegment>(snakeSegmentPrefab, this.transform.parent);
    }

    public void StartSpawnRoutine(Vector3 direction, List<SnakeSegment> segments)
    {
        _coroutine ??= StartCoroutine(SpawnRoutine(direction, segments));
    }

    private void EndSpawn()
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }
    }

    private IEnumerator SpawnRoutine(Vector3 direction, List<SnakeSegment> segments)
    {
        Queue<CubeStack> stacks = new(_cubeStorage.GetStacks());
        Vector3 centerPoint;

        bool isWork = true;

        while (isWork)
        {
            if (segments.Count - _targetStorage.Count < 10 && stacks.Count > 0)
            {
                var stack = stacks.Dequeue();
                int segmentCount = stack.Count / 4;

                if (stack == null)
                    Debug.Log("Stack is null");

                if (stack.Count == 0)
                    Debug.Log($"{stack.Count} - {stack.Material} - {stack.Material.color}");


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
                            cube.InitSegment(segment);

                            segment.AddCube(cube);
                        }

                        segment.SnakeMover.InitLengths((segment.transform.localScale.magnitude + _lengthMultiplier) / 2, segment.transform.localScale.magnitude / 2);
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
            }

            yield return null;
        }
    }
}