using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class SplineRoad : MonoBehaviour
{
    [Header("Road Settings")]
    [SerializeField] private float _roadWidth = 3.3f;
    [SerializeField] private float _textureTiling = 4f;
    [SerializeField] private Material _roadMaterial;
    [SerializeField] private MeshFilter _meshFilter;
    [SerializeField] private MeshRenderer _meshRenderer;

    [Header("End Platform Settings")]
    [SerializeField] private int _platformSegments = 16;

    [Header("Quality Settings")]
    [Range(50, 500)] public int _resolution = 200;

    private float _endPlatformRadius;
    private SplineContainer _splineContainer;
    private Mesh _roadMesh;

    private void Awake()
    {
        _endPlatformRadius = _roadWidth / 2;
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

        for (int i = 0; i <= _resolution; i++)
        {
            float t = i / (float)_resolution;

            spline.Evaluate(t, out float3 position, out float3 tangent, out float3 upVector);

            Vector3 roadTangentNormalized = new Vector3(tangent.x, tangent.y, tangent.z).normalized;
            Vector3 roadUpNormalized = new Vector3(upVector.x, upVector.y, upVector.z).normalized;
            Vector3 right = Vector3.Cross(roadTangentNormalized, roadUpNormalized).normalized;

            float widthMultiplier = 1f;

            Vector3 leftEdge = new Vector3(position.x, position.y, position.z) - 0.5f * _roadWidth * widthMultiplier * right;
            Vector3 rightEdge = new Vector3(position.x, position.y, position.z) + 0.5f * _roadWidth * widthMultiplier * right;

            vertices.Add(leftEdge);
            vertices.Add(rightEdge);

            float uvY = t * _textureTiling;
            uv.Add(new Vector2(0f, uvY));
            uv.Add(new Vector2(1f, uvY));

            normals.Add(roadUpNormalized);
            normals.Add(roadUpNormalized);
        }

        for (int i = 0; i < _resolution; i++)
        {
            int currentLeft = i * 2;
            int currentRight = i * 2 + 1;
            int nextLeft = (i + 1) * 2;
            int nextRight = (i + 1) * 2 + 1;

            triangles.Add(currentLeft);
            triangles.Add(currentRight);
            triangles.Add(nextLeft);

            triangles.Add(currentRight);
            triangles.Add(nextRight);
            triangles.Add(nextLeft);
        }

        float platformYOffset = -0.001f;

        _splineContainer.Evaluate(1f, out float3 endPosition, out float3 endTangent, out float3 endUp);
        Vector3 platformTangentNormalized = new Vector3(endTangent.x, endTangent.y, endTangent.z).normalized;
        Vector3 platformUpNormalized = new Vector3(endUp.x, endUp.y, endUp.z).normalized;
        Vector3 center = new Vector3(endPosition.x, endPosition.y, endPosition.z) + Vector3.up * platformYOffset;

        int centerIndex = vertices.Count;
        vertices.Add(center);
        uv.Add(new Vector2(0.5f, 0.5f));
        normals.Add(platformUpNormalized);

        float startAngle = Mathf.Atan2(platformTangentNormalized.z, platformTangentNormalized.x) + Mathf.PI * 0.5f;

        for (int i = 0; i <= _platformSegments; i++)
        {
            float angle = startAngle + i / (float)_platformSegments * Mathf.PI * 2f;
            Vector3 direction = new(Mathf.Cos(angle), 0, Mathf.Sin(angle));
            Vector3 platformEdge = center + direction * _endPlatformRadius;

            vertices.Add(platformEdge);

            float uvX = 0.5f + Mathf.Cos(angle) * 0.5f;
            float uvY = 0.5f + Mathf.Sin(angle) * 0.5f;
            uv.Add(new Vector2(uvX, uvY));
            normals.Add(platformUpNormalized);
        }

        for (int i = 1; i < _platformSegments; i++)
        {
            triangles.Add(centerIndex);
            triangles.Add(centerIndex + i + 1);
            triangles.Add(centerIndex + i);
        }

        triangles.Add(centerIndex);
        triangles.Add(centerIndex + 1);
        triangles.Add(centerIndex + _platformSegments);

        int lastRoadLeft = (_resolution) * 2;
        int lastRoadRight = (_resolution) * 2 + 1;

        for (int i = 0; i < _platformSegments; i++)
        {
            int platformVertex1 = centerIndex + i + 1;
            int platformVertex2 = centerIndex + i + 2;

            if (i < _platformSegments / 2)
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
