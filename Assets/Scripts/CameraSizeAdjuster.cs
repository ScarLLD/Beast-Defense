using UnityEngine;

public class CameraSizeAdjuster : MonoBehaviour
{
    [SerializeField] private Camera targetCamera;
    [SerializeField] private Transform leftBoundaryObject;
    [SerializeField] private Transform rightBoundaryObject;
    [SerializeField] private Transform bottomBoundaryObject;

    private void Start()
    {        
        FitCameraToBoundaries();
    }

    public void FitCameraToBoundaries()
    {
        if (targetCamera == null || !targetCamera.orthographic)
            return;

        if (leftBoundaryObject == null || rightBoundaryObject == null)
            return;

        Vector3 leftInCameraSpace = targetCamera.transform.InverseTransformPoint(leftBoundaryObject.position);
        Vector3 rightInCameraSpace = targetCamera.transform.InverseTransformPoint(rightBoundaryObject.position);

        float leftBoundary = leftInCameraSpace.x;
        float rightBoundary = rightInCameraSpace.x;
        float requiredWidth = Mathf.Abs(rightBoundary - leftBoundary);

        float aspect = targetCamera.aspect;
        float requiredOrthoSizeForWidth = (requiredWidth / aspect) / 2f;
        float requiredOrthoSize = requiredOrthoSizeForWidth;

        if (bottomBoundaryObject != null)
        {
            Vector3 bottomInCameraSpace = targetCamera.transform.InverseTransformPoint(bottomBoundaryObject.position);
            float bottomBoundary = bottomInCameraSpace.y;

            float requiredOrthoSizeForBottom = Mathf.Abs(bottomBoundary);
            requiredOrthoSize = Mathf.Max(requiredOrthoSizeForWidth, requiredOrthoSizeForBottom);
        }

        targetCamera.orthographicSize = requiredOrthoSize;
    }
}