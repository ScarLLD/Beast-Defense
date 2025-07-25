using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SnakeHead))]
public class SnakeTail : MonoBehaviour
{
    private readonly int _maxActiveSegmentsCount = 20;
    private readonly float _lengthMultiplier = 0f;
    private readonly float _distanceBetweenSegments = 0.8f;
    private readonly float _distanceBetweenCubes = 0.5f;
    private readonly float _scaleMultiplier = 0.7f;
    private readonly float _sizeDivider = 2;
    private readonly float _DirectionMultiplier = 1f;
    private Coroutine _coroutine;

    private int _currentActiveSegmentsCount = 0;
    private SnakeHead _snakeHead;
    private CubeStorage _cubeStorage;
    private Cube _cubePrefab;

    private ObjectPool<SnakeSegment> _pool;

    private void Awake()
    {
        _snakeHead = GetComponent<SnakeHead>();
    }

    public void Init(CubeStorage cubeStorage, Cube cubePrefab, SnakeSegment snakeSegmentPrefab)
    {
        _cubeStorage = cubeStorage;
        _cubePrefab = cubePrefab;

        _pool = new ObjectPool<SnakeSegment>(snakeSegmentPrefab, this.transform.parent);
    }

    public void StartSpawn(Vector3 direction)
    {
        _coroutine = StartCoroutine(Spawn(direction));
    }

    private IEnumerator Spawn(Vector3 direction)
    {
        bool isWork = true;
        Queue<ICube> stacks = new(_cubeStorage.Stacks);
        ICube tempCube = null;
        Transform lastSegment = null; // Для хранения ссылки на последний сегмент
        Vector3 centerPoint;

        while (isWork)
        {
            if (_currentActiveSegmentsCount < _maxActiveSegmentsCount && stacks.Count > 0)
            {
                var stack = stacks.Dequeue();
                int segmentCount = stack.Count / 4;

                for (int i = 0; i < segmentCount; i++)
                {
                    // Обновляем centerPoint перед созданием нового сегмента
                    if (lastSegment != null)
                    {
                        centerPoint = lastSegment.position + -direction.normalized * GetObjectSizeInLocalDirection(-direction) / _sizeDivider * _distanceBetweenSegments;
                    }
                    else
                    {
                        centerPoint = transform.position + _DirectionMultiplier * _distanceBetweenSegments * GetObjectSizeInLocalDirection(-direction) * -direction.normalized;
                    }

                    var segment = _pool.GetObject();
                    segment.Init(_snakeHead);
                    segment.transform.position = centerPoint;                    
                    lastSegment = segment.transform;
                    transform.rotation = Quaternion.LookRotation(lastSegment.transform.position - transform.position * 90f);

                    Vector3[] points = new Vector3[4];
                    Vector3 rightOffset = transform.right * _distanceBetweenCubes;
                    Vector3 upOffset = transform.up * _distanceBetweenCubes;

                    points[0] = centerPoint + rightOffset + upOffset;
                    points[1] = centerPoint - rightOffset + upOffset;
                    points[2] = centerPoint - rightOffset - upOffset;
                    points[3] = centerPoint + rightOffset - upOffset;

                    for (int l = 0; l < points.Length; l++)
                    {
                        Cube cube = Instantiate(_cubePrefab, points[l], Quaternion.identity, segment.transform);
                        cube.transform.localScale *= _scaleMultiplier;
                        cube.Init(stack.Material);
                        cube.GetSegment(segment);

                        segment.AddCube(cube);
                    }

                    segment.SnakeMover.SetLengths((segment.transform.localScale.magnitude + _lengthMultiplier) / 2, segment.transform.localScale.magnitude / 2);

                    _currentActiveSegmentsCount++;
                    tempCube = stack;
                }
            }

            yield return null;
        }
    }

    public void DecreaseActiveSegmentsCount()
    {
        if (_currentActiveSegmentsCount > 0)
            _currentActiveSegmentsCount--;
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