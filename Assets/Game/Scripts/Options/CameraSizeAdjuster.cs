using UnityEngine;

namespace Game.Scripts.Options
{
    public class CameraSizeAdjuster : MonoBehaviour
    {
        [SerializeField] private Camera _targetCamera;
        [SerializeField] private Transform _leftBoundaryObject;
        [SerializeField] private Transform _rightBoundaryObject;
        [SerializeField] private Transform _bottomBoundaryObject;

        private void Start()
        {
            FitCameraToBoundaries();
        }

        private void FitCameraToBoundaries()
        {
            if (!_targetCamera || !_targetCamera.orthographic)
                return;

            if (!_leftBoundaryObject || !_rightBoundaryObject)
                return;

            var leftInCameraSpace = _targetCamera.transform.InverseTransformPoint(_leftBoundaryObject.position);
            var rightInCameraSpace = _targetCamera.transform.InverseTransformPoint(_rightBoundaryObject.position);

            var leftBoundary = leftInCameraSpace.x;
            var rightBoundary = rightInCameraSpace.x;
            var requiredWidth = Mathf.Abs(rightBoundary - leftBoundary);

            var aspect = _targetCamera.aspect;
            var requiredOrthoSizeForWidth = (requiredWidth / aspect) / 2f;
            var requiredOrthoSize = requiredOrthoSizeForWidth;

            if (_bottomBoundaryObject)
            {
                var bottomInCameraSpace = _targetCamera.transform.InverseTransformPoint(_bottomBoundaryObject.position);
                var bottomBoundary = bottomInCameraSpace.y;

                var requiredOrthoSizeForBottom = Mathf.Abs(bottomBoundary);
                requiredOrthoSize = Mathf.Max(requiredOrthoSizeForWidth, requiredOrthoSizeForBottom);
            }

            _targetCamera.orthographicSize = requiredOrthoSize;
        }
    }
}