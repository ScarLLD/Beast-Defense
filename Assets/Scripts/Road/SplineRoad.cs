using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class SplineRoad : MonoBehaviour
{
    [Header("Road Settings")]
    public float roadWidth = 6f;
    public float textureTiling = 4f;
    public Material roadMaterial;
    public Material endPlatformMaterial;

    [Header("End Platform Settings")]
    public float endPlatformRadius = 4f;
    public int _platformSegments = 16;

    [Header("Quality Settings")]
    [Range(50, 500)] public int resolution = 200;
    public float endRoundness = 0.5f;

    private SplineContainer _splineContainer;
    private MeshFilter _meshFilter;
    private MeshRenderer _meshRenderer;
    private Mesh _roadMesh;

    public bool TryGenerateRoadFromSpline(SplineContainer splineContainer)
    {
        _splineContainer = splineContainer;

        if (_splineContainer == null)
            return false;

        if (_meshFilter == null) _meshFilter = gameObject.AddComponent<MeshFilter>();
        if (_meshRenderer == null) _meshRenderer = gameObject.AddComponent<MeshRenderer>();

        SetupRoadMaterial();
        GenerateSmoothRoadMesh();

        return true;
    }

    private void SetupRoadMaterial()
    {
        if (roadMaterial != null)
        {
            _meshRenderer.material = roadMaterial;
            _meshRenderer.material.mainTextureScale = new Vector2(1f, textureTiling);
        }
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

        for (int i = 0; i <= resolution; i++)
        {
            float t = i / (float)resolution;

            spline.Evaluate(t, out float3 position, out float3 tangent, out float3 upVector);

            Vector3 tangentNormalized = ((Vector3)tangent).normalized;
            Vector3 upNormalized = ((Vector3)upVector).normalized;
            Vector3 right = Vector3.Cross(tangentNormalized, upNormalized).normalized;

            float widthMultiplier = 1f;

            Vector3 leftEdge = (Vector3)position - 0.5f * roadWidth * widthMultiplier * right;
            Vector3 rightEdge = (Vector3)position + 0.5f * roadWidth * widthMultiplier * right;

            vertices.Add(leftEdge);
            vertices.Add(rightEdge);

            float uvY = t * textureTiling;
            uv.Add(new Vector2(0f, uvY));
            uv.Add(new Vector2(1f, uvY));

            normals.Add(upNormalized);
            normals.Add(upNormalized);
        }

        for (int i = 0; i < resolution; i++)
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

        AddEndPlatform();

        _roadMesh.vertices = vertices.ToArray();
        _roadMesh.triangles = triangles.ToArray();
        _roadMesh.uv = uv.ToArray();
        _roadMesh.normals = normals.ToArray();

        _roadMesh.RecalculateBounds();
        _roadMesh.Optimize();

        _meshFilter.mesh = _roadMesh;
    }

    private void AddEndPlatform()
    {
        float platformYOffset = -0.001f;

        List<Vector3> vertices = new();
        List<Vector2> uv = new();
        List<Vector3> normals = new();
        List<int> triangles = new();

        _splineContainer.Evaluate(1f, out float3 endPosition, out float3 endTangent, out float3 endUp);

        Vector3 tangentNormalized = ((Vector3)endTangent).normalized;
        Vector3 upNormalized = ((Vector3)endUp).normalized;

        Vector3 center = (Vector3)endPosition;
        int centerIndex = 0;

        vertices.Add(center);
        uv.Add(new Vector2(0.5f, 0.5f));
        normals.Add(upNormalized);

        float startAngle = Mathf.Atan2(tangentNormalized.z, tangentNormalized.x) + Mathf.PI * 0.5f;

        for (int i = 0; i <= _platformSegments; i++)
        {
            float angle = startAngle + i / (float)_platformSegments * Mathf.PI * 2f;
            Vector3 direction = new(Mathf.Cos(angle), 0, Mathf.Sin(angle));
            Vector3 platformEdge = center + direction * endPlatformRadius;

            vertices.Add(platformEdge);

            float uvX = 0.5f + Mathf.Cos(angle) * 0.5f;
            float uvY = 0.5f + Mathf.Sin(angle) * 0.5f;
            uv.Add(new Vector2(uvX, uvY));
            normals.Add(upNormalized);
        }

        for (int i = 1; i < _platformSegments; i++)
        {
            triangles.Add(centerIndex);
            triangles.Add(i + 1);
            triangles.Add(i);
        }

        triangles.Add(centerIndex);
        triangles.Add(1);
        triangles.Add(_platformSegments);

        Mesh platformMesh = new()
        {
            name = "EndPlatform"
        };

        platformMesh.SetVertices(vertices);
        platformMesh.SetUVs(0, uv);
        platformMesh.SetNormals(normals);
        platformMesh.SetTriangles(triangles, 0);
        platformMesh.RecalculateBounds();

        GameObject platformObj = new("EndPlatform");
        platformObj.transform.SetParent(this.transform);

        platformObj.transform.SetLocalPositionAndRotation(Vector3.up * platformYOffset, Quaternion.identity);
        platformObj.transform.localScale = Vector3.one;

        var meshFilter = platformObj.AddComponent<MeshFilter>();
        var meshRenderer = platformObj.AddComponent<MeshRenderer>();

        meshFilter.mesh = platformMesh;
        meshRenderer.material = endPlatformMaterial != null ? endPlatformMaterial : new Material(Shader.Find("Standard"));
    }

    public void ClearRoad()
    {
        if (_meshFilter != null && _meshFilter.mesh != null)
        {
            DestroyImmediate(_meshFilter.mesh);
        }

        Transform platformTransform = transform.Find("EndPlatform");
        if (platformTransform != null)
        {
            DestroyImmediate(platformTransform.gameObject);
        }
    }

    void OnDestroy()
    {
        ClearRoad();
    }
}
