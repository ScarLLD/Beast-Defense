using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace Game.Scripts.Road
{
    public class SplineVisualizer : MonoBehaviour
    {
        [SerializeField] private Material _roadMaterial;
        [SerializeField] private float _roadWidth = 3.3f;
        [SerializeField] private int _platformSegments = 16;
        [SerializeField] private int _roadQualitySegments = 240;

        private float _endPlatformRadius;
        private MeshFilter _meshFilter;
        private SplineContainer _splineContainer;
        private Mesh _roadMesh;

        private void Awake()
        {
            _endPlatformRadius = _roadWidth / 2;
            _meshFilter = GetComponent<MeshFilter>();
        }

        public bool TryGenerateRoadFromSpline(SplineContainer splineContainer)
        {
            if (_splineContainer)
                _splineContainer.RemoveSpline(_splineContainer.Spline);

            _splineContainer = splineContainer;

            if (!_splineContainer)
                return false;

            GenerateSmoothRoadMesh();

            return true;
        }

        private void GenerateSmoothRoadMesh()
        {
            if (!_splineContainer) return;

            _roadMesh = new Mesh
            {
                name = "RoadMesh",
            };

            List<Vector3> vertices = new ();
            List<int> triangles = new ();
            List<Vector2> uv = new ();
            List<Vector3> normals = new ();

            var spline = _splineContainer.Spline;

            for (var segmentIndex = 0; segmentIndex <= _roadQualitySegments; segmentIndex++)
            {
                var splinePosition = segmentIndex / (float)_roadQualitySegments;

                spline.Evaluate(splinePosition, out var position, out var tangent, out var upVector);

                var roadTangent = new Vector3(tangent.x, tangent.y, tangent.z).normalized;
                var roadUp = new Vector3(upVector.x, upVector.y, upVector.z).normalized;
                var roadRight = Vector3.Cross(roadTangent, roadUp).normalized;

                const float widthMultiplier = 1f;

                var leftEdge = new Vector3(position.x, position.y, position.z) - 0.5f * _roadWidth * widthMultiplier * roadRight;
                var rightEdge = new Vector3(position.x, position.y, position.z) + 0.5f * _roadWidth * widthMultiplier * roadRight;

                vertices.Add(leftEdge);
                vertices.Add(rightEdge);

                uv.Add(new Vector2(0f, splinePosition));
                uv.Add(new Vector2(1f, splinePosition));

                normals.Add(roadUp);
                normals.Add(roadUp);
            }

            for (var segmentIndex = 0; segmentIndex < _roadQualitySegments; segmentIndex++)
            {
                var currentLeft = segmentIndex * 2;
                var currentRight = segmentIndex * 2 + 1;
                var nextLeft = (segmentIndex + 1) * 2;
                var nextRight = (segmentIndex + 1) * 2 + 1;

                triangles.Add(currentLeft);
                triangles.Add(currentRight);
                triangles.Add(nextLeft);

                triangles.Add(currentRight);
                triangles.Add(nextRight);
                triangles.Add(nextLeft);
            }

            const float platformYOffset = -0.001f;

            _splineContainer.Evaluate(1f, out var endPosition, out var endTangent, out var endUp);
            var platformTangent = new Vector3(endTangent.x, endTangent.y, endTangent.z).normalized;
            var platformUp = new Vector3(endUp.x, endUp.y, endUp.z).normalized;
            var platformCenter = new Vector3(endPosition.x, endPosition.y, endPosition.z) + Vector3.up * platformYOffset;

            var centerIndex = vertices.Count;
            vertices.Add(platformCenter);
            uv.Add(new Vector2(0.5f, 0.5f));
            normals.Add(platformUp);

            var startAngle = Mathf.Atan2(platformTangent.z, platformTangent.x) + Mathf.PI * 0.5f;

            for (var segmentIndex = 0; segmentIndex <= _platformSegments; segmentIndex++)
            {
                var angle = startAngle + segmentIndex / (float)_platformSegments * Mathf.PI * 2f;
                Vector3 direction = new(Mathf.Cos(angle), 0, Mathf.Sin(angle));
                var platformEdge = platformCenter + direction * _endPlatformRadius;

                vertices.Add(platformEdge);

                var uvHorizontal = 0.5f + Mathf.Cos(angle) * 0.5f;
                var uvVertical = 0.5f + Mathf.Sin(angle) * 0.5f;
                uv.Add(new Vector2(uvHorizontal, uvVertical));
                normals.Add(platformUp);
            }

            for (var segmentIndex = 1; segmentIndex < _platformSegments; segmentIndex++)
            {
                triangles.Add(centerIndex);
                triangles.Add(centerIndex + segmentIndex + 1);
                triangles.Add(centerIndex + segmentIndex);
            }

            triangles.Add(centerIndex);
            triangles.Add(centerIndex + 1);
            triangles.Add(centerIndex + _platformSegments);

            var lastRoadLeft = _roadQualitySegments * 2;
            var lastRoadRight = _roadQualitySegments * 2 + 1;

            for (var segmentIndex = 0; segmentIndex < _platformSegments; segmentIndex++)
            {
                var platformVertex1 = centerIndex + segmentIndex + 1;
                var platformVertex2 = centerIndex + segmentIndex + 2;

                if (segmentIndex < _platformSegments / 2)
                {
                    triangles.Add(lastRoadLeft);
                    triangles.Add(platformVertex1);
                    triangles.Add(platformVertex2);

                    triangles.Add(lastRoadLeft);
                    triangles.Add(platformVertex2);
                    triangles.Add(platformVertex1);
                }
                else
                {
                    triangles.Add(lastRoadRight);
                    triangles.Add(platformVertex2);
                    triangles.Add(platformVertex1);

                    triangles.Add(lastRoadRight);
                    triangles.Add(platformVertex1);
                    triangles.Add(platformVertex2);
                }
            }

            _roadMesh.vertices = vertices.ToArray();
            _roadMesh.triangles = triangles.ToArray();
            _roadMesh.uv = uv.ToArray();
            _roadMesh.normals = normals.ToArray();

            _roadMesh.RecalculateBounds();
            _roadMesh.Optimize();

            _meshFilter.mesh = _roadMesh;
        }

        private void ClearRoad()
        {
            if (_meshFilter != null && _meshFilter.mesh != null)
            {
                DestroyImmediate(_meshFilter.mesh);
            }
        }

        private void OnDestroy()
        {
            ClearRoad();
        }
    }
}