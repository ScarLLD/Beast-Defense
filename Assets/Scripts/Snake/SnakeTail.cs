using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(SnakeSegmentsHolder))]
public class SnakeTail : MonoBehaviour
{
    private int _tailCubesCount = 10;
    private float _distanceBetweenSegments = 1.2f;
    private float _distanceBetweenCubes = 0.5f;
    private float _sizeDivider = 2;
    private float _DirectionMultiplier = 0.8f;
    private Vector3 _lastPosition;
    private SnakeSegmentsHolder _holder;

    private void Awake()
    {
        _lastPosition = transform.position;
        _holder = GetComponent<SnakeSegmentsHolder>();
    }

    public void Spawn(Vector3 direction, SnakeHead snakeHead, PathHolder pathHolder)
    {
        Vector3 centerPoint = _lastPosition + _DirectionMultiplier * _distanceBetweenSegments * GetObjectSizeInLocalDirection(-direction) * -direction.normalized;

        for (int i = 0; i < _tailCubesCount; i++)
        {
            GameObject segmentObject = new GameObject();
            SnakeSegment segment = segmentObject.AddComponent<SnakeSegment>();
            segment.transform.position = centerPoint;
            segment.transform.parent = snakeHead.transform.parent;
            segment.Init(snakeHead, pathHolder);

            Vector3[] points = new Vector3[4];

            Vector3 rightOffset = transform.right * _distanceBetweenCubes;
            Vector3 upOffset = transform.up * _distanceBetweenCubes;

            points[0] = centerPoint + rightOffset + upOffset;
            points[1] = centerPoint - rightOffset + upOffset;
            points[2] = centerPoint - rightOffset - upOffset;
            points[3] = centerPoint + rightOffset - upOffset;

            for (int l = 0; l < points.Length; l++)
            {
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.position = points[l];
                cube.transform.localScale = Vector3.one * 0.7f;
                cube.transform.parent = segment.transform;                
            }

            _holder.AddSegment(segment);

            _lastPosition = centerPoint;
            centerPoint = _lastPosition + -direction.normalized * GetObjectSizeInLocalDirection(-direction) / _sizeDivider * _distanceBetweenSegments;
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
}
