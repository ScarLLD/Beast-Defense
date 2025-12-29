using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

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
        if (_splineContainer != null)
            _splineContainer.RemoveSpline(_splineContainer.Spline);

        _splineContainer = splineContainer;

        if (_splineContainer == null)
            return false;

        GenerateSmoothRoadMesh();

        return true;
    }

    private void GenerateSmoothRoadMesh()
    {
        if (_splineContainer == null) return;

        _roadMesh = new Mesh
        {
            name = "RoadMesh"
        };

        List<Vector3> vertices = new();
        List<int> triangles = new();
        List<Vector2> uv = new();
        List<Vector3> normals = new();

        Spline spline = _splineContainer.Spline;

        for (int segmentIndex = 0; segmentIndex <= _roadQualitySegments; segmentIndex++)
        {
            float splinePosition = segmentIndex / (float)_roadQualitySegments;

            spline.Evaluate(splinePosition, out float3 position, out float3 tangent, out float3 upVector);

            Vector3 roadTangent = new Vector3(tangent.x, tangent.y, tangent.z).normalized;
            Vector3 roadUp = new Vector3(upVector.x, upVector.y, upVector.z).normalized;
            Vector3 roadRight = Vector3.Cross(roadTangent, roadUp).normalized;

            float widthMultiplier = 1f;

            Vector3 leftEdge = new Vector3(position.x, position.y, position.z) - 0.5f * _roadWidth * widthMultiplier * roadRight;
            Vector3 rightEdge = new Vector3(position.x, position.y, position.z) + 0.5f * _roadWidth * widthMultiplier * roadRight;

            vertices.Add(leftEdge);
            vertices.Add(rightEdge);

            float uvVertical = splinePosition;
            uv.Add(new Vector2(0f, uvVertical));
            uv.Add(new Vector2(1f, uvVertical));

            normals.Add(roadUp);
            normals.Add(roadUp);
        }

        for (int segmentIndex = 0; segmentIndex < _roadQualitySegments; segmentIndex++)
        {
            int currentLeft = segmentIndex * 2;
            int currentRight = segmentIndex * 2 + 1;
            int nextLeft = (segmentIndex + 1) * 2;
            int nextRight = (segmentIndex + 1) * 2 + 1;

            triangles.Add(currentLeft);
            triangles.Add(currentRight);
            triangles.Add(nextLeft);

            triangles.Add(currentRight);
            triangles.Add(nextRight);
            triangles.Add(nextLeft);
        }

        float platformYOffset = -0.001f;

        _splineContainer.Evaluate(1f, out float3 endPosition, out float3 endTangent, out float3 endUp);
        Vector3 platformTangent = new Vector3(endTangent.x, endTangent.y, endTangent.z).normalized;
        Vector3 platformUp = new Vector3(endUp.x, endUp.y, endUp.z).normalized;
        Vector3 platformCenter = new Vector3(endPosition.x, endPosition.y, endPosition.z) + Vector3.up * platformYOffset;

        int centerIndex = vertices.Count;
        vertices.Add(platformCenter);
        uv.Add(new Vector2(0.5f, 0.5f));
        normals.Add(platformUp);

        float startAngle = Mathf.Atan2(platformTangent.z, platformTangent.x) + Mathf.PI * 0.5f;

        for (int segmentIndex = 0; segmentIndex <= _platformSegments; segmentIndex++)
        {
            float angle = startAngle + segmentIndex / (float)_platformSegments * Mathf.PI * 2f;
            Vector3 direction = new(Mathf.Cos(angle), 0, Mathf.Sin(angle));
            Vector3 platformEdge = platformCenter + direction * _endPlatformRadius;

            vertices.Add(platformEdge);

            float uvHorizontal = 0.5f + Mathf.Cos(angle) * 0.5f;
            float uvVertical = 0.5f + Mathf.Sin(angle) * 0.5f;
            uv.Add(new Vector2(uvHorizontal, uvVertical));
            normals.Add(platformUp);
        }

        for (int segmentIndex = 1; segmentIndex < _platformSegments; segmentIndex++)
        {
            triangles.Add(centerIndex);
            triangles.Add(centerIndex + segmentIndex + 1);
            triangles.Add(centerIndex + segmentIndex);
        }

        triangles.Add(centerIndex);
        triangles.Add(centerIndex + 1);
        triangles.Add(centerIndex + _platformSegments);

        int lastRoadLeft = _roadQualitySegments * 2;
        int lastRoadRight = _roadQualitySegments * 2 + 1;

        for (int segmentIndex = 0; segmentIndex < _platformSegments; segmentIndex++)
        {
            int platformVertex1 = centerIndex + segmentIndex + 1;
            int platformVertex2 = centerIndex + segmentIndex + 2;

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

    public void ClearRoad()
    {
        if (_meshFilter != null && _meshFilter.mesh != null)
        {
            DestroyImmediate(_meshFilter.mesh);
        }
    }

    void OnDestroy()
    {
        ClearRoad();
    }
}