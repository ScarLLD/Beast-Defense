using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DirectionAnalyzer))]
public class RoadLimiter : MonoBehaviour
{
    [SerializeField] private float _boundaryMargin = 1f;
    [SerializeField] private float _radiusBetweenSegments = 1.5f;
    [SerializeField] private float _endPointMargin = 3f;
    [SerializeField] private BoundaryMaker _boundaryMaker;

    private DirectionAnalyzer _directionHolder;
    private float _leftBoundX;
    private float _rightBoundX;
    private float _lowerBoundZ;
    private float _upperBoundZ;

    private void Start()
    {
        _directionHolder = GetComponent<DirectionAnalyzer>();

        if (_boundaryMaker != null)
        {
            UpdateBoundariesFromBoundaryMaker();
        }
        else if (_directionHolder != null)
        {
            _leftBoundX = _directionHolder.LeftBoundX;
            _rightBoundX = _directionHolder.RightBoundX;
            _lowerBoundZ = _directionHolder.LowerBoundZ;
            _upperBoundZ = _directionHolder.UpperBoundZ;
        }
        else
        {
            _leftBoundX = -10f;
            _rightBoundX = 10f;
            _lowerBoundZ = -10f;
            _upperBoundZ = 10f;
        }
    }

    public void UpdateBoundariesFromBoundaryMaker()
    {
        if (_boundaryMaker == null) return;

        if (_boundaryMaker.TryGetBoundaryLimits(out float minX, out float maxX, out float minZ, out float maxZ))
        {
            _leftBoundX = minX;
            _rightBoundX = maxX;
            _lowerBoundZ = minZ;
            _upperBoundZ = maxZ;
        }
    }

    public bool IsTooCloseToBoundary(Vector3 point)
    {
        return point.x < _leftBoundX + _boundaryMargin ||
               point.x > _rightBoundX - _boundaryMargin ||
               point.z > _upperBoundZ - _boundaryMargin ||
               point.z < _lowerBoundZ + _boundaryMargin;
    }

    public bool IsEndTooCloseToBoundary(Vector3 point)
    {
        return point.x < _leftBoundX + _boundaryMargin * _endPointMargin ||
               point.x > _rightBoundX - _boundaryMargin * _endPointMargin ||
               point.z > _upperBoundZ - _boundaryMargin * _endPointMargin ||
               point.z < _lowerBoundZ + _boundaryMargin * _endPointMargin;
    }

    public bool IsPositionValid(Vector3 position, List<Vector3> pathPoints)
    {
        if (IsTooCloseToBoundary(position))
        {
            return false;
        }

        if (pathPoints != null)
        {
            foreach (var point in pathPoints)
            {
                if (Vector3.Distance(position, point) < _radiusBetweenSegments)
                {
                    return false;
                }
            }
        }

        return true;
    }

    public void SetBoundaries(float leftX, float rightX, float lowerZ, float upperZ)
    {
        _leftBoundX = leftX;
        _rightBoundX = rightX;
        _lowerBoundZ = lowerZ;
        _upperBoundZ = upperZ;
    }

    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;

        Gizmos.color = Color.yellow;

        Vector3 topLeft = new Vector3(_leftBoundX + _boundaryMargin, 0, _upperBoundZ - _boundaryMargin);
        Vector3 topRight = new Vector3(_rightBoundX - _boundaryMargin, 0, _upperBoundZ - _boundaryMargin);
        Gizmos.DrawLine(topLeft, topRight);

        Vector3 bottomLeft = new Vector3(_leftBoundX + _boundaryMargin, 0, _lowerBoundZ + _boundaryMargin);
        Vector3 bottomRight = new Vector3(_rightBoundX - _boundaryMargin, 0, _lowerBoundZ + _boundaryMargin);
        Gizmos.DrawLine(bottomLeft, bottomRight);

        Gizmos.DrawLine(topLeft, bottomLeft);
        Gizmos.DrawLine(topRight, bottomRight);
    }
}