using System.Collections.Generic;
using System.Linq;
using Game.Scripts.MapGenerator;
using UnityEngine;

namespace Game.Scripts.Road
{
    [RequireComponent(typeof(DirectionAnalyzer))]
    public class RoadLimiter : MonoBehaviour
    {
        [SerializeField] private float _boundaryMargin = 1f;
        [SerializeField] private float _radiusBetweenSegments = 1.5f;
        [SerializeField] private float _endPointMargin = 3f;
        [SerializeField] private BoundaryMaker _boundaryMaker;

        private DirectionAnalyzer _directionHolder;
        private float _leftBoundX;
        private float _lowerBoundZ;
        private float _rightBoundX;
        private float _upperBoundZ;

        private void Start()
        {
            _directionHolder = GetComponent<DirectionAnalyzer>();

            if (_boundaryMaker)
            {
                UpdateBoundariesFromBoundaryMaker();
            }
            else if (_directionHolder)
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

        private void OnDrawGizmosSelected()
        {
            if (!Application.isPlaying) return;

            Gizmos.color = Color.yellow;

            Vector3 topLeft = new(_leftBoundX + _boundaryMargin, 0, _upperBoundZ - _boundaryMargin);
            Vector3 topRight = new(_rightBoundX - _boundaryMargin, 0, _upperBoundZ - _boundaryMargin);
            Gizmos.DrawLine(topLeft, topRight);

            Vector3 bottomLeft = new(_leftBoundX + _boundaryMargin, 0, _lowerBoundZ + _boundaryMargin);
            Vector3 bottomRight = new(_rightBoundX - _boundaryMargin, 0, _lowerBoundZ + _boundaryMargin);
            Gizmos.DrawLine(bottomLeft, bottomRight);

            Gizmos.DrawLine(topLeft, bottomLeft);
            Gizmos.DrawLine(topRight, bottomRight);
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
            if (IsTooCloseToBoundary(position)) return false;

            return pathPoints == null ||
                   pathPoints.All(point => !(Vector3.Distance(position, point) < _radiusBetweenSegments));
        }

        private void UpdateBoundariesFromBoundaryMaker()
        {
            if (!_boundaryMaker) return;

            if (!_boundaryMaker.TryGetBoundaryLimits(out var minX, out var maxX, out var minZ, out var maxZ)) return;
            _leftBoundX = minX;
            _rightBoundX = maxX;
            _lowerBoundZ = minZ;
            _upperBoundZ = maxZ;
        }

        private bool IsTooCloseToBoundary(Vector3 point)
        {
            return point.x < _leftBoundX + _boundaryMargin ||
                   point.x > _rightBoundX - _boundaryMargin ||
                   point.z > _upperBoundZ - _boundaryMargin ||
                   point.z < _lowerBoundZ + _boundaryMargin;
        }
    }
}