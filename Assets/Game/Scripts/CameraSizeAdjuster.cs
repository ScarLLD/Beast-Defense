using UnityEngine;

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

    public void FitCameraToBoundaries()
    {
        if (_targetCamera == null || !_targetCamera.orthographic)
            return;

        if (_leftBoundaryObject == null || _rightBoundaryObject == null)
            return;

        Vector3 leftInCameraSpace = _targetCamera.transform.InverseTransformPoint(_leftBoundaryObject.position);
        Vector3 rightInCameraSpace = _targetCamera.transform.InverseTransformPoint(_rightBoundaryObject.position);

        float leftBoundary = leftInCameraSpace.x;
        float rightBoundary = rightInCameraSpace.x;
        float requiredWidth = Mathf.Abs(rightBoundary - leftBoundary);

        float aspect = _targetCamera.aspect;
        float requiredOrthoSizeForWidth = (requiredWidth / aspect) / 2f;
        float requiredOrthoSize = requiredOrthoSizeForWidth;

        if (_bottomBoundaryObject != null)
        {
            Vector3 bottomInCameraSpace = _targetCamera.transform.InverseTransformPoint(_bottomBoundaryObject.position);
            float bottomBoundary = bottomInCameraSpace.y;

            float requiredOrthoSizeForBottom = Mathf.Abs(bottomBoundary);
            requiredOrthoSize = Mathf.Max(requiredOrthoSizeForWidth, requiredOrthoSizeForBottom);
        }

        _targetCamera.orthographicSize = requiredOrthoSize;
    }
}