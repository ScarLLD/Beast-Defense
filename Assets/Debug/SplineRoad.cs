using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class SplineRoad : MonoBehaviour
{
    [Header("Road Settings")]
    [SerializeField] private Material _roadMaterial;
    [SerializeField] private float _roadWidth = 6f;
    [SerializeField] private float _textureTiling = 4f;

    [Header("End Platform Settings")]
    [SerializeField] private float _endPlatformRadius = 4f;
    [SerializeField] private int _platformSegments = 16;

    [Header("Quality Settings")]
    [Range(50, 500)]
    [SerializeField] private int _resolution = 200;

    private SplineContainer _splineContainer;
    private MeshFilter _meshFilter;
    private MeshRenderer _meshRenderer;
    private Mesh _roadMesh;

    private void Awake()
    {
        transform.position = new Vector3(0, 0.001f, 0);
    }

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

    void SetupRoadMaterial()
    {
        if (_roadMaterial != null)
        {
            _meshRenderer.material = _roadMaterial;
            _meshRenderer.material.mainTextureScale = new Vector2(1f, _textureTiling);
        }
        else
        {
            _meshRenderer.material = new Material(Shader.Find("Standard"));
            _meshRenderer.material.color = new Color(0.2f, 0.2f, 0.2f);
            _meshRenderer.material.mainTextureScale = new Vector2(1f, _textureTiling);
        }

        _meshRenderer.material.doubleSidedGI = true;
    }

    void GenerateSmoothRoadMesh()
    {
        if (_splineContainer == null) return;

        _roadMesh = new Mesh
        {
            name = "RoadMesh"
        };

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uv = new List<Vector2>();
        List<Vector3> normals = new List<Vector3>();

        Spline spline = _splineContainer.Spline;

        // Основная часть дороги
        for (int i = 0; i <= _resolution; i++)
        {
            float t = i / (float)_resolution;

            spline.Evaluate(t, out float3 position, out float3 tangent, out float3 upVector);

            Vector3 tangentNormalized = ((Vector3)tangent).normalized;
            Vector3 upNormalized = ((Vector3)upVector).normalized;
            Vector3 right = Vector3.Cross(tangentNormalized, upNormalized).normalized;

            Vector3 leftEdge = (Vector3)position - right * _roadWidth * 0.5f;
            Vector3 rightEdge = (Vector3)position + right * _roadWidth * 0.5f;

            vertices.Add(leftEdge);
            vertices.Add(rightEdge);

            float uvY = t * _textureTiling;
            uv.Add(new Vector2(0f, uvY));
            uv.Add(new Vector2(1f, uvY));

            normals.Add(upNormalized);
            normals.Add(upNormalized);
        }

        // Создаем треугольники для дороги
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

        // Добавляем круглую площадку в конце
        AddEndPlatform(vertices, uv, normals, triangles);

        // Применяем данные к mesh
        _roadMesh.vertices = vertices.ToArray();
        _roadMesh.triangles = triangles.ToArray();
        _roadMesh.uv = uv.ToArray();
        _roadMesh.normals = normals.ToArray();

        _roadMesh.RecalculateBounds();
        _roadMesh.Optimize();

        _meshFilter.mesh = _roadMesh;
    }

    void AddEndPlatform(List<Vector3> vertices, List<Vector2> uv, List<Vector3> normals, List<int> triangles)
    {
        // Получаем данные конечной точки
        _splineContainer.Evaluate(1f, out float3 endPosition, out float3 endTangent, out float3 endUp);

        Vector3 tangentNormalized = ((Vector3)endTangent).normalized;
        Vector3 upNormalized = ((Vector3)endUp).normalized;
        Vector3 right = Vector3.Cross(tangentNormalized, upNormalized).normalized;

        int centerIndex = vertices.Count;
        Vector3 center = (Vector3)endPosition;

        // Центральная точка площадки
        vertices.Add(center);
        uv.Add(new Vector2(0.5f, 0.5f));
        normals.Add(upNormalized);

        // Крайние точки дороги в конце
        Vector3 roadLeft = (Vector3)endPosition - right * _roadWidth * 0.5f;
        Vector3 roadRight = (Vector3)endPosition + right * _roadWidth * 0.5f;

        // Определяем начальный угол (перпендикулярно направлению дороги)
        float startAngle = Mathf.Atan2(tangentNormalized.z, tangentNormalized.x) + Mathf.PI * 0.5f;

        // Создаем круг из вершин
        for (int i = 0; i <= _platformSegments; i++)
        {
            float angle = startAngle + i / (float)_platformSegments * Mathf.PI * 2f;

            // Направление от центра
            Vector3 direction = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
            Vector3 platformEdge = center + direction * _endPlatformRadius;

            vertices.Add(platformEdge);

            // UV координаты для круга
            float uvX = 0.5f + Mathf.Cos(angle) * 0.5f;
            float uvY = 0.5f + Mathf.Sin(angle) * 0.5f;
            uv.Add(new Vector2(uvX, uvY));

            normals.Add(upNormalized);
        }

        // Находим индексы точек, которые соответствуют левому и правому краю дороги
        int leftEdgeIndex = -1;
        int rightEdgeIndex = -1;
        float minLeftDist = float.MaxValue;
        float minRightDist = float.MaxValue;

        for (int i = 1; i <= _platformSegments; i++)
        {
            int vertexIndex = centerIndex + i;
            float distToLeft = Vector3.Distance(vertices[vertexIndex], roadLeft);
            float distToRight = Vector3.Distance(vertices[vertexIndex], roadRight);

            if (distToLeft < minLeftDist)
            {
                minLeftDist = distToLeft;
                leftEdgeIndex = vertexIndex;
            }

            if (distToRight < minRightDist)
            {
                minRightDist = distToRight;
                rightEdgeIndex = vertexIndex;
            }
        }

        // Соединяем дорогу с площадкой (левая сторона)
        int lastRoadLeft = _resolution * 2;
        if (leftEdgeIndex != -1)
        {
            triangles.Add(lastRoadLeft);
            triangles.Add(leftEdgeIndex);
            triangles.Add(centerIndex);
        }

        // Соединяем дорогу с площадкой (правая сторона)
        int lastRoadRight = _resolution * 2 + 1;
        if (rightEdgeIndex != -1)
        {
            triangles.Add(lastRoadRight);
            triangles.Add(centerIndex);
            triangles.Add(rightEdgeIndex);
        }

        // Заполняем круглую площадку треугольниками
        for (int i = 1; i < _platformSegments; i++)
        {
            triangles.Add(centerIndex);
            triangles.Add(centerIndex + i + 1);
            triangles.Add(centerIndex + i);
        }

        // Замыкаем круг
        triangles.Add(centerIndex);
        triangles.Add(centerIndex + 1);
        triangles.Add(centerIndex + _platformSegments);
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