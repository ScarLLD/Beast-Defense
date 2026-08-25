using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.MapGenerator
{
    public class BoundaryMaker : MonoBehaviour
    {
        private readonly Color _spawnAreaColor = Color.yellow;
        
        [SerializeField] private List<Transform> _customBoundaryPoints;
        [SerializeField] private List<BoundarySegment> _manualSegments;

        [Header("Gizmos Settings")]
        [SerializeField] private float _lineWidth = 0.1f;
        [SerializeField] private bool _drawGizmos = true;

        private List<LineSegment> _lineSegments;
        private Dictionary<BoundarySide, List<LineSegment>> _segmentsBySide;

        public enum BoundarySide
        {
            Left,
            Right,
            Top,
            Bottom,
            Custom
        }

        private void Awake()
        {
            _lineSegments = new List<LineSegment>();
            _segmentsBySide = new Dictionary<BoundarySide, List<LineSegment>>();
            InitializeManualBoundaries();
        }

        public void InitializeManualBoundaries()
        {
            _lineSegments.Clear();
            _segmentsBySide.Clear();

            if (_manualSegments != null && _manualSegments.Count > 0)
            {
                foreach (var segment in _manualSegments)
                {
                    if (segment == null) continue;

                    if (segment.StartPoint == null || segment.EndPoint == null) continue;
                    
                    var lineSegment = new LineSegment(
                        segment.StartPoint.position,
                        segment.EndPoint.position,
                        segment.Side,
                        segment.SpawnMinOffset,
                        segment.SpawnMaxOffset);

                    _lineSegments.Add(lineSegment);

                    if (!_segmentsBySide.ContainsKey(segment.Side))
                    {
                        _segmentsBySide[segment.Side] = new List<LineSegment>();
                    }

                    _segmentsBySide[segment.Side].Add(lineSegment);
                }
            }

            if (_customBoundaryPoints == null || _customBoundaryPoints.Count < 2) return;
            {
                for (var i = 0; i < _customBoundaryPoints.Count - 1; i++)
                {
                    if (_customBoundaryPoints[i] == null || _customBoundaryPoints[i + 1] == null) continue;
                    
                    var lineSegment = new LineSegment(
                        _customBoundaryPoints[i].position,
                        _customBoundaryPoints[i + 1].position,
                        BoundarySide.Custom);

                    _lineSegments.Add(lineSegment);

                    if (!_segmentsBySide.ContainsKey(BoundarySide.Custom))
                    {
                        _segmentsBySide[BoundarySide.Custom] = new List<LineSegment>();
                    }

                    _segmentsBySide[BoundarySide.Custom].Add(lineSegment);
                }
            }
        }

        public Vector3 GetRandomPointOnSide(BoundarySide side)
        {
            if (!_segmentsBySide.ContainsKey(side) || _segmentsBySide[side].Count == 0)
            {
                return Vector3.zero;
            }

            var segments = _segmentsBySide[side];
            var segmentIndex = Random.Range(0, segments.Count);
            var segment = segments[segmentIndex];

            return segment.GetRandomPoint();
        }

        public bool TryGetBoundaryLimits(out float minX, out float maxX, out float minZ, out float maxZ)
        {
            minX = float.MaxValue;
            maxX = float.MinValue;
            minZ = float.MaxValue;
            maxZ = float.MinValue;

            if (_lineSegments.Count == 0)
            {
                return false;
            }

            foreach (var segment in _lineSegments)
            {
                minX = Mathf.Min(minX, segment.Start.x, segment.End.x);
                maxX = Mathf.Max(maxX, segment.Start.x, segment.End.x);
                minZ = Mathf.Min(minZ, segment.Start.z, segment.End.z);
                maxZ = Mathf.Max(maxZ, segment.Start.z, segment.End.z);
            }

            return true;
        }

        public BoundarySide GetRandomSide()
        {
            if (_segmentsBySide.Count == 0)
            {
                return BoundarySide.Custom;
            }

            List<BoundarySide> availableSides = new(_segmentsBySide.Keys);

            if (availableSides.Count > 1 && availableSides.Contains(BoundarySide.Custom))
            {
                availableSides.Remove(BoundarySide.Custom);
            }

            if (availableSides.Count == 0)
            {
                return BoundarySide.Custom;
            }

            var index = Random.Range(0, availableSides.Count);
            return availableSides[index];
        }

        private void OnDrawGizmos()
        {
            if (!_drawGizmos || _lineSegments == null || _lineSegments.Count == 0)
                return;

            foreach (var segment in _lineSegments)
            {
                Gizmos.color = Color.green;

                Gizmos.DrawLine(segment.Start, segment.End);
                Gizmos.DrawSphere(segment.Start, _lineWidth * 0.5f);
                Gizmos.DrawSphere(segment.End, _lineWidth * 0.5f);

                Gizmos.color = _spawnAreaColor;
                Vector3 spawnStart = Vector3.Lerp(segment.Start, segment.End, segment.SpawnMinOffset);
                Vector3 spawnEnd = Vector3.Lerp(segment.Start, segment.End, segment.SpawnMaxOffset);
                Gizmos.DrawLine(spawnStart, spawnEnd);
                Gizmos.DrawSphere(spawnStart, _lineWidth * 0.3f);
                Gizmos.DrawSphere(spawnEnd, _lineWidth * 0.3f);
            }
        }

        private struct LineSegment
        {
            public Vector3 Start;
            public Vector3 End;
            public BoundarySide Side;
            public float SpawnMinOffset;
            public float SpawnMaxOffset;

            public LineSegment(Vector3 start, Vector3 end, BoundarySide side, float minOffset = 0.3f, float maxOffset = 0.7f)
            {
                Start = start;
                End = end;
                Side = side;
                SpawnMinOffset = minOffset;
                SpawnMaxOffset = maxOffset;
            }

            public readonly Vector3 GetRandomPoint()
            {
                float randomT = Random.Range(SpawnMinOffset, SpawnMaxOffset);
                return Vector3.Lerp(Start, End, randomT);
            }
        }

        [System.Serializable]
        public class BoundarySegment
        {
            public Transform StartPoint;
            public Transform EndPoint;
            public BoundarySide Side;
            public float SpawnMinOffset = 0.3f;
            public float SpawnMaxOffset = 0.7f;
        }
    }
}