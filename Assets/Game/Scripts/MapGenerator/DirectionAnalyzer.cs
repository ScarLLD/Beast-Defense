using UnityEngine;

namespace Game.Scripts.MapGenerator
{
    public class DirectionAnalyzer : MonoBehaviour
    {
        [SerializeField] private BoundaryMaker _boundaryMaker;

        public float LeftBoundX { get; private set; }
        public float RightBoundX { get; private set; }
        public float UpperBoundZ { get; private set; }
        public float LowerBoundZ { get; private set; }

        private void Start()
        {
            if (_boundaryMaker)
            {
                UpdateBoundaries();
            }
            else
            {
                LeftBoundX = -10f;
                RightBoundX = 10f;
                UpperBoundZ = 10f;
                LowerBoundZ = -10f;
            }
        }

        public Vector3 GetValidDirection(Vector3 point)
        {
            Vector3 direction;

            var distToLeft = Mathf.Abs(point.x - LeftBoundX);
            var distToRight = Mathf.Abs(point.x - RightBoundX);
            var distToTop = Mathf.Abs(point.z - UpperBoundZ);
            var distToBottom = Mathf.Abs(point.z - LowerBoundZ);

            var minDist = Mathf.Min(distToLeft, distToRight, distToTop, distToBottom);

            if (Mathf.Approximately(minDist, distToLeft))
                direction = Vector3.right;
            else if (Mathf.Approximately(minDist, distToRight))
                direction = Vector3.left;
            else if (Mathf.Approximately(minDist, distToTop))
                direction = Vector3.back;
            else
                direction = Vector3.forward;

            return direction;
        }
        
        private void UpdateBoundaries()
        {
            if (!_boundaryMaker ||
                !_boundaryMaker.TryGetBoundaryLimits(out var minX, out var maxX, out var minZ, out var maxZ))
                return;
            
            LeftBoundX = minX;
            RightBoundX = maxX;
            UpperBoundZ = maxZ;
            LowerBoundZ = minZ;
        }
    }
}