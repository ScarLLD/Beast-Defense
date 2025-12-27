using UnityEngine;

public class DirectionAnalyzer : MonoBehaviour
{
    [SerializeField] private BoundaryMaker _boundaryMaker;

    private float _leftBoundX;
    private float _rightBoundX;
    private float _upperBoundZ;
    private float _lowerBoundZ;

    public float LeftBoundX => _leftBoundX;
    public float RightBoundX => _rightBoundX;
    public float UpperBoundZ => _upperBoundZ;
    public float LowerBoundZ => _lowerBoundZ;

    private void Start()
    {
        if (_boundaryMaker != null)
        {
            UpdateBoundaries();
        }
        else
        {
            _leftBoundX = -10f;
            _rightBoundX = 10f;
            _upperBoundZ = 10f;
            _lowerBoundZ = -10f;
        }
    }

    public void UpdateBoundaries()
    {
        if (_boundaryMaker != null && _boundaryMaker.TryGetBoundaryLimits(out float minX, out float maxX, out float minZ, out float maxZ))
        {
            _leftBoundX = minX;
            _rightBoundX = maxX;
            _upperBoundZ = maxZ;
            _lowerBoundZ = minZ;
        }
    }

    public Vector3 GetValidDirection(Vector3 point)
    {
        Vector3 direction = Vector3.zero;

        float distToLeft = Mathf.Abs(point.x - _leftBoundX);
        float distToRight = Mathf.Abs(point.x - _rightBoundX);
        float distToTop = Mathf.Abs(point.z - _upperBoundZ);
        float distToBottom = Mathf.Abs(point.z - _lowerBoundZ);

        float minDist = Mathf.Min(distToLeft, distToRight, distToTop, distToBottom);

        if (minDist == distToLeft)
            direction = Vector3.right;
        else if (minDist == distToRight)
            direction = Vector3.left;
        else if (minDist == distToTop)
            direction = Vector3.back;
        else
            direction = Vector3.forward;

        return direction;
    }
}