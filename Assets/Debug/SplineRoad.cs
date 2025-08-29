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

    void SetupRoadMaterial()
    {
        if (roadMaterial != null)
        {
            _meshRenderer.material = roadMaterial;
            _meshRenderer.material.mainTextureScale = new Vector2(1f, textureTiling);
        }
    }

    void GenerateSmoothRoadMesh()
    {
        if (_splineContainer == null) return;

        _roadMesh = new Mesh();
        _roadMesh.name = "RoadMesh";

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uv = new List<Vector2>();
        List<Vector3> normals = new List<Vector3>();

        Spline spline = _splineContainer.Spline;

        // Основная часть дороги
        for (int i = 0; i <= resolution; i++)
        {
            float t = i / (float)resolution;

            spline.Evaluate(t, out float3 position, out float3 tangent, out float3 upVector);

            Vector3 tangentNormalized = ((Vector3)tangent).normalized;
            Vector3 upNormalized = ((Vector3)upVector).normalized;
            Vector3 right = Vector3.Cross(tangentNormalized, upNormalized).normalized;

            // Закругление только в конце
            float widthMultiplier = 1f;
            if (t > 0.91f) // Только последние 10% дороги
            {
                widthMultiplier = Mathf.SmoothStep(1f, 0f, (t - 0.91f) / 0.1f) * endRoundness;
            }

            Vector3 leftEdge = (Vector3)position - right * roadWidth * 0.5f * widthMultiplier;
            Vector3 rightEdge = (Vector3)position + right * roadWidth * 0.5f * widthMultiplier;

            vertices.Add(leftEdge);
            vertices.Add(rightEdge);

            // UV координаты
            float uvY = t * textureTiling;
            uv.Add(new Vector2(0f, uvY));
            uv.Add(new Vector2(1f, uvY));

            // Нормали (вверх)
            normals.Add(upNormalized);
            normals.Add(upNormalized);
        }

        // Создаем треугольники ПРАВИЛЬНОГО порядка (по часовой стрелке)
        for (int i = 0; i < resolution; i++)
        {
            int currentLeft = i * 2;
            int currentRight = i * 2 + 1;
            int nextLeft = (i + 1) * 2;
            int nextRight = (i + 1) * 2 + 1;

            // Первый треугольник (по часовой стрелке)
            triangles.Add(currentLeft);
            triangles.Add(currentRight);
            triangles.Add(nextLeft);

            // Второй треугольник (по часовой стрелке)
            triangles.Add(currentRight);
            triangles.Add(nextRight);
            triangles.Add(nextLeft);
        }

        // Добавляем конечное закругление
        AddEndCap(vertices, uv, normals, triangles);

        // Добавляем отдельную платформу (отдельный GameObject и Mesh)
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

    void AddEndPlatform(List<Vector3> roadVertices, List<Vector2> roadUV, List<Vector3> roadNormals, List<int> roadTriangles)
    {
        // Смещение платформы вниз относительно дороги
        float platformYOffset = -0.001f;

        // Создание списков для платформы
        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uv = new List<Vector2>();
        List<Vector3> normals = new();
        List<int> triangles = new();

        _splineContainer.Evaluate(1f, out float3 endPosition, out float3 endTangent, out float3 endUp);

        Vector3 tangentNormalized = ((Vector3)endTangent).normalized;
        Vector3 upNormalized = ((Vector3)endUp).normalized;
        Vector3 right = Vector3.Cross(tangentNormalized, upNormalized).normalized;

        Vector3 center = (Vector3)endPosition;
        int centerIndex = 0;

        vertices.Add(center);
        uv.Add(new Vector2(0.5f, 0.5f));
        normals.Add(upNormalized);

        float startAngle = Mathf.Atan2(tangentNormalized.z, tangentNormalized.x) + Mathf.PI * 0.5f;

        for (int i = 0; i <= _platformSegments; i++)
        {
            float angle = startAngle + i / (float)_platformSegments * Mathf.PI * 2f;
            Vector3 direction = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
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

        // Замыкающий треугольник
        triangles.Add(centerIndex);
        triangles.Add(1);
        triangles.Add(_platformSegments);

        // Создание и заполнение Mesh
        Mesh platformMesh = new();
        platformMesh.name = "EndPlatform";
        platformMesh.SetVertices(vertices);
        platformMesh.SetUVs(0, uv);
        platformMesh.SetNormals(normals);
        platformMesh.SetTriangles(triangles, 0);
        platformMesh.RecalculateBounds();

        // Создание GameObject платформы
        GameObject platformObj = new("EndPlatform");
        platformObj.transform.SetParent(this.transform);

        // Смещение вниз
        platformObj.transform.localPosition = Vector3.up * platformYOffset;
        platformObj.transform.localRotation = Quaternion.identity;
        platformObj.transform.localScale = Vector3.one;

        var meshFilter = platformObj.AddComponent<MeshFilter>();
        var meshRenderer = platformObj.AddComponent<MeshRenderer>();

        meshFilter.mesh = platformMesh;
        meshRenderer.material = endPlatformMaterial != null ? endPlatformMaterial : new Material(Shader.Find("Standard"));
    }

    void AddEndCap(List<Vector3> vertices, List<Vector2> uv, List<Vector3> normals, List<int> triangles)
    {
        // Получаем данные конечной точки
        _splineContainer.Evaluate(1f, out float3 endPosition, out float3 endTangent, out float3 endUp);

        Vector3 tangentNormalized = ((Vector3)endTangent).normalized;
        Vector3 upNormalized = ((Vector3)endUp).normalized;
        Vector3 right = Vector3.Cross(tangentNormalized, upNormalized).normalized;

        int centerIndex = vertices.Count;
        Vector3 center = (Vector3)endPosition;

        // Центральная точка конца
        vertices.Add(center);
        uv.Add(new Vector2(0.5f, 1f));
        normals.Add(upNormalized);

        // Вершины для закругления (полукруг)
        int segments = 8;
        for (int i = 0; i <= segments; i++)
        {
            float angle = i / (float)segments * Mathf.PI;
            float x = Mathf.Cos(angle) * roadWidth * 0.5f * endRoundness;
            float z = Mathf.Sin(angle) * roadWidth * 0.5f * endRoundness;

            Vector3 offset = right * x + tangentNormalized * z;
            Vector3 capVertex = center + offset;

            vertices.Add(capVertex);
            uv.Add(new Vector2(i / (float)segments, 1f));
            normals.Add(upNormalized);
        }

        // Соединяем закругление с последним сегментом дороги (по часовой стрелке)
        int lastRoadLeft = (resolution) * 2;
        int lastRoadRight = (resolution) * 2 + 1;

        for (int i = 0; i < segments; i++)
        {
            triangles.Add(lastRoadLeft);
            triangles.Add(centerIndex + i + 2);
            triangles.Add(centerIndex + i + 1);
        }

        // Заполняем полукруг (по часовой стрелке)
        for (int i = 0; i < segments; i++)
        {
            triangles.Add(centerIndex);
            triangles.Add(centerIndex + i + 2);
            triangles.Add(centerIndex + i + 1);
        }
    }

    public void ClearRoad()
    {
        if (_meshFilter != null && _meshFilter.mesh != null)
        {
            DestroyImmediate(_meshFilter.mesh);
        }

        // Удаляем платформу, если есть
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
