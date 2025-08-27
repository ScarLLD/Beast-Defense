using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RoadSpawner))]
public class DirectionAnalyzer : MonoBehaviour
{
    [SerializeField] private BoundaryMaker _boundaryMaker;

    public float LeftBoundX { get; private set; }
    public float RightBoundX { get; private set; }
    public float UpperBoundZ { get; private set; }
    public float LowerBoundZ { get; private set; }

    public bool IsLeftBound(Vector3 point) => CompareDifference(point.x, LeftBoundX);
    public bool IsRightBound(Vector3 point) => CompareDifference(point.x, RightBoundX);
    public bool IsUpperBound(Vector3 point) => CompareDifference(point.z, UpperBoundZ);

    private void OnEnable()
    {
        _boundaryMaker.PointsInitialized += InitBounds;
    }

    private void OnDisable()
    {
        _boundaryMaker.PointsInitialized -= InitBounds;
    }

    public void InitBounds(List<Vector3> _points)
    {
        LeftBoundX = _points[0].x;
        UpperBoundZ = _points[1].z;
        RightBoundX = _points[2].x;
        LowerBoundZ = _points[3].z * 0.25f;
    }

    public Vector3 GetValidDirection(Vector3 point)
    {
        if (IsLeftBound(point))
            return Vector3.right;
        else if (IsUpperBound(point))
            return Vector3.back;
        else if (IsRightBound(point))
            return Vector3.left;

        return Vector3.zero;
    }

    private bool CompareDifference(float firstNumber, float secondNumber)
    {
        float distance = firstNumber - secondNumber;
        return distance < 0.1f && distance > -0.1f;
    }
}
