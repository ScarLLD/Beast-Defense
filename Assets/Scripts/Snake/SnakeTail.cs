using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(SnakeSegmentsStorage))]
public class SnakeTail : MonoBehaviour
{
    private float _distanceBetweenSegments = 0.8f;
    private float _distanceBetweenCubes = 0.5f;
    private float _sizeDivider = 2;
    private float _DirectionMultiplier = 1f;
    private Vector3 _lastPosition;

    private SnakeSegmentsStorage _segmentsStorage;
    private CubeStorage _cubeStorage;
    private Cube _cubePrefab;
    private SnakeSegment _snakeSegmentPrefab;

    private void Awake()
    {
        _lastPosition = transform.position;
        _segmentsStorage = GetComponent<SnakeSegmentsStorage>();
    }

    public void Init(CubeStorage cubeStorage, Cube cubePrefab, SnakeSegment snakeSegmentPrefab)
    {
        _cubeStorage = cubeStorage;
        _cubePrefab = cubePrefab;
        _snakeSegmentPrefab = snakeSegmentPrefab;
    }

    public void Spawn(Vector3 direction, SnakeHead snakeHead, PathHolder pathHolder)
    {
        int remained—ountInsideStack = 0;

        Vector3 centerPoint = _lastPosition + _DirectionMultiplier * _distanceBetweenSegments * GetObjectSizeInLocalDirection(-direction) * -direction.normalized;

        for (int i = 0; i < _cubeStorage.Stacks.Count;)
        {
            int countInsideStack = _cubeStorage.Stacks[i].Count - remained—ountInsideStack;

            SnakeSegment snakeSegment = Instantiate(_snakeSegmentPrefab, centerPoint, Quaternion.identity, snakeHead.transform.parent);
            snakeSegment.Init(snakeHead, pathHolder);

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
                cube.transform.localScale *= 0.7f;
                cube.Init(_cubeStorage.Stacks[i].Material);
                cube.GetSegment(snakeSegment);

                snakeSegment.AddCube(cube);

                remained—ountInsideStack++;
            }

            _segmentsStorage.AddSegment(snakeSegment);

            _lastPosition = centerPoint;
            centerPoint = _lastPosition + -direction.normalized * GetObjectSizeInLocalDirection(-direction) / _sizeDivider * _distanceBetweenSegments;

            if (countInsideStack == 0)
            {
                remained—ountInsideStack = 0;
                i++;
            }
        }
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

    public void ProcessLoss(SnakeSegment snakeSegment)
    {
        if (_segmentsStorage.TryGetSeveredSegments(snakeSegment, out List<SnakeSegment> severedSegments))
        {
            foreach (var segments in severedSegments)
            {

            }
        }
    }
}
