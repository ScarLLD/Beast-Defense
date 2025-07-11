using System.Collections.Generic;
using UnityEngine;

public class SnakeTail : MonoBehaviour
{
    private readonly float _distanceBetweenSegments = 0.8f;
    private readonly float _distanceBetweenCubes = 0.5f;
    private readonly float _scaleMultiplier = 0.7f;
    private readonly float _sizeDivider = 2;
    private readonly float _DirectionMultiplier = 1f;
    private Vector3 _lastPosition;

    private CubeStorage _cubeStorage;
    private Cube _cubePrefab;
    private SnakeSegment _snakeSegmentPrefab;

    private void Awake()
    {
        _lastPosition = transform.position;
    }

    public void Init(CubeStorage cubeStorage, Cube cubePrefab, SnakeSegment snakeSegmentPrefab)
    {
        _cubeStorage = cubeStorage;
        _cubePrefab = cubePrefab;
        _snakeSegmentPrefab = snakeSegmentPrefab;
    }

    public bool TryCreateSegments(Vector3 direction, out List<SnakeSegment> segments, out float scaleMultiplier)
    {
        segments = new();
        scaleMultiplier = _scaleMultiplier;

        Vector3 centerPoint = _lastPosition + _DirectionMultiplier * _distanceBetweenSegments * GetObjectSizeInLocalDirection(-direction) * -direction.normalized;

        for (int i = 0; i < _cubeStorage.Stacks.Count; i++)
        {
            for (int j = 0; j < _cubeStorage.Stacks[i].Count / 4; j++)
            {
                SnakeSegment snakeSegment = Instantiate(_snakeSegmentPrefab, centerPoint, Quaternion.identity);

                Vector3[] points = new Vector3[4];

                Vector3 rightOffset = transform.right * _distanceBetweenCubes;
                Vector3 upOffset = transform.up * _distanceBetweenCubes;

                points[0] = centerPoint + rightOffset + upOffset;
                points[1] = centerPoint - rightOffset + upOffset;
                points[2] = centerPoint - rightOffset - upOffset;
                points[3] = centerPoint + rightOffset - upOffset;

                for (int l = 0; l < points.Length; l++)
                {
                    Cube cube = Instantiate(_cubePrefab, points[l], Quaternion.identity, snakeSegment.transform);
                    cube.transform.localScale *= scaleMultiplier;
                    cube.Init(_cubeStorage.Stacks[i].Material);
                    cube.GetSegment(snakeSegment);

                    snakeSegment.AddCube(cube);
                }

                segments.Add(snakeSegment);

                _lastPosition = centerPoint;
                centerPoint = _lastPosition + -direction.normalized * GetObjectSizeInLocalDirection(-direction) / _sizeDivider * _distanceBetweenSegments;
            }
        }

        return segments.Count != 0;
    }

    public float GetObjectSizeInLocalDirection(Vector3 localDirection)
    {
        Collider collider = GetComponent<Collider>();
        if (collider == null) return 0f;

        Vector3 worldDirection = transform.TransformDirection(localDirection.normalized);

        Bounds bounds = collider.bounds;
        float thickness = Vector3.Dot(bounds.size, worldDirection);

        return Mathf.Abs(thickness);
    }
}
