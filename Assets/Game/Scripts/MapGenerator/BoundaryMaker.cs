using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Scripts.MapGenerator
{
    public class BoundaryMaker : MonoBehaviour
    {
        private readonly Color _spawnAreaColor = Color.yellow;

        [SerializeField] private List<Transform> _customBoundaryPoints;
        [SerializeField] private List<BoundarySegment> _manualSegments;

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

            if (_manualSegments is { Count: > 0 })
            {
                foreach (var segment in _manualSegments)
                {
                    if (segment == null) continue;

                    if (!segment.StartPoint || !segment.EndPoint) continue;

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

            for (var i = 0; i < _customBoundaryPoints.Count - 1; i++)
            {
                if (!_customBoundaryPoints[i] || !_customBoundaryPoints[i + 1]) continue;

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

        private struct LineSegment
        {
            private readonly float _spawnMinOffset;
            private readonly float _spawnMaxOffset;

            public Vector3 Start;
            public Vector3 End;
            public BoundarySide Side;

            public LineSegment(Vector3 start, Vector3 end, BoundarySide side, float minOffset = 0.3f,
                float maxOffset = 0.7f)
            {
                Start = start;
                End = end;
                Side = side;
                _spawnMinOffset = minOffset;
                _spawnMaxOffset = maxOffset;
            }

            public readonly Vector3 GetRandomPoint()
            {
                var randomT = Random.Range(_spawnMinOffset, _spawnMaxOffset);
                return Vector3.Lerp(Start, End, randomT);
            }
        }

        [Serializable]
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