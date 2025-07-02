using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DirectionAnalyzer))]
public class RoadLimiter : MonoBehaviour
{
    [SerializeField] private float _boundaryMargin = 1f;
    [SerializeField] private float _radiusBetweenSegments = 1.5f;
    [SerializeField] private float _endPointMargin = 3f;

    private DirectionAnalyzer _directionHolder;

    private void Awake()
    {
        _directionHolder = GetComponent<DirectionAnalyzer>();
    }

    public bool IsTooCloseToBoundary(Vector3 point)
    {
        return point.x < _directionHolder.LeftBoundX + _boundaryMargin ||
               point.x > _directionHolder.RightBoundX - _boundaryMargin ||
               point.z > _directionHolder.UpperBoundZ - _boundaryMargin ||
               point.z < _directionHolder.LowerBoundZ + _boundaryMargin;
    }

    public bool IsEndTooCloseToBoundary(Vector3 point)
    {
        return point.x < _directionHolder.LeftBoundX + _boundaryMargin * _endPointMargin ||
               point.x > _directionHolder.RightBoundX - _boundaryMargin * _endPointMargin ||
               point.z > _directionHolder.UpperBoundZ - _boundaryMargin * _endPointMargin ||
               point.z < _directionHolder.LowerBoundZ + _boundaryMargin * _endPointMargin;
    }

    public bool IsPositionValid(Vector3 position, List<Vector3> pathPoints)
    {
        if (IsTooCloseToBoundary(position))
            return false;

        foreach (var point in pathPoints)
        {
            if (Vector3.Distance(position, point) < _radiusBetweenSegments)
                return false;
        }

        return true;
    }
}
