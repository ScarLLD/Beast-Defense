using System;
using UnityEngine;
using System.Collections.Generic;

public class BoundaryMaker : MonoBehaviour
{
    [SerializeField] private Transform _container;

    [Range(0.6f, 0.84f)]
    [SerializeField] private float _borderZLowerReduction = 0.7f;

    [Range(0.85f, 0.98f)]
    [SerializeField] private float _borderZUpperReduction = 0.9f;

    [Header("Gizmos Settings")]
    [SerializeField] private Color _lineColor = Color.green;
    [SerializeField] private float _lineWidth = 0.1f;
    [SerializeField] private bool _drawGizmos = true;
    [SerializeField] private float _gizmosHeight = 0.1f;

    private List<Vector3> _planePoints;
    private List<LineSegment> _lineSegments;
    private Camera _camera;

    public event Action<List<Vector3>> PointsInitialized;

    private struct LineSegment
    {
        public Vector3 Start;
        public Vector3 End;

        public LineSegment(Vector3 start, Vector3 end)
        {
            Start = start;
            End = end;
        }

        public readonly Vector3 GetRandomPoint(float minT = 0.3f, float maxT = 0.7f)
        {
            float randomT = UnityEngine.Random.Range(minT, maxT);
            return Vector3.Lerp(Start, End, randomT);
        }
    }

    private void Awake()
    {
        _planePoints = new List<Vector3>();
        _lineSegments = new List<LineSegment>();
        _camera = Camera.main;
    }

    public Vector3 GetRandomPointOnRandomLine()
    {
        if (_lineSegments.Count == 0) return Vector3.zero;

        int index = UnityEngine.Random.Range(0, _lineSegments.Count);
        var segment = _lineSegments[index];

        return segment.GetRandomPoint(0.3f, 0.7f);
    }

    public bool TryGetScreenBottomCenter(out Vector3 bottomScreenCenter)
    {
        bottomScreenCenter = Vector3.zero;

        Vector3 screenPoint = new(_camera.pixelWidth * 0.5f, _camera.pixelHeight * 0.25f, 0f);

        Ray ray = _camera.ScreenPointToRay(screenPoint);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            bottomScreenCenter = hit.point;
            return true;
        }

        return false;
    }

    public bool TryGetScreenWidthBounds(out float minX, out float maxX, float gridExtraWidth = 0f)
    {
        minX = 0f;
        maxX = 0f;

        if (_camera == null)
            return false;

        Vector3 leftScreen = _camera.ViewportToWorldPoint(new Vector3(0, 0.5f, _camera.nearClipPlane));
        Vector3 rightScreen = _camera.ViewportToWorldPoint(new Vector3(1, 0.5f, _camera.nearClipPlane));

        minX = leftScreen.x - gridExtraWidth / 2f;
        maxX = rightScreen.x + gridExtraWidth / 2f;

        return true;
    }

    public bool TryGetWorldPointFromScreenPoint(Vector3 screenPoint, out Vector3 worldPoint)
    {
        worldPoint = Vector3.zero;

        if (_camera == null)
            return false;

        Ray ray = _camera.ScreenPointToRay(screenPoint);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            worldPoint = hit.point;
            return true;
        }

        return false;
    }

    public bool TryGeneratePathMarkers()
    {
        if (_camera == null)
            return false;

        float lowerHeight = _camera.pixelHeight * _borderZLowerReduction;
        float upperHeight = _camera.pixelHeight * _borderZUpperReduction;

        List<Vector3> screenPoints = new()
        {
            new Vector3(0f, lowerHeight, 10),
            new Vector3(0f, upperHeight, 10),

            new Vector3(_camera.pixelWidth, lowerHeight, 10),
            new Vector3(_camera.pixelWidth, upperHeight, 10),

            new Vector3(0f, _camera.pixelHeight, 10),
            new Vector3(_camera.pixelWidth, _camera.pixelHeight, 10)
        };

        _planePoints.Clear();
        _lineSegments.Clear();

        foreach (Vector3 screenPoint in screenPoints)
        {
            if (TryGetWorldPointFromScreenPoint(screenPoint, out Vector3 worldPoint))
            {               
                worldPoint.y += _gizmosHeight;
                _planePoints.Add(worldPoint);
            }
        }

        CreateBorderSegments();
        PointsInitialized?.Invoke(_planePoints);

        if (_planePoints.Count == 0)
            Debug.Log("Не удалось сгенерировать грань карты.");

        return _planePoints.Count > 0;
    }

    private void CreateBorderSegments()
    {
        if (_planePoints.Count < 2) return;

        for (int i = 0; i < _planePoints.Count - 1; i += 2)
        {
            if (i + 1 < _planePoints.Count)
            {
                LineSegment segment = new(_planePoints[i], _planePoints[i + 1]);
                _lineSegments.Add(segment);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (!_drawGizmos || _lineSegments == null || _lineSegments.Count == 0)
            return;

        Gizmos.color = _lineColor;

        foreach (var segment in _lineSegments)
        {
            Gizmos.DrawLine(segment.Start, segment.End);

            Gizmos.DrawSphere(segment.Start, _lineWidth * 0.5f);
            Gizmos.DrawSphere(segment.End, _lineWidth * 0.5f);
        }

        Gizmos.color = Color.yellow;
        foreach (var point in _planePoints)
        {
            Gizmos.DrawWireSphere(point, _lineWidth * 0.3f);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!_drawGizmos || _lineSegments == null || _lineSegments.Count == 0)
            return;

        Gizmos.color = new Color(_lineColor.r, _lineColor.g, _lineColor.b, 0.5f);

        foreach (var segment in _lineSegments)
        {
            Vector3 direction = (segment.End - segment.Start).normalized;
            Vector3 perpendicular = _lineWidth * 0.5f * Vector3.Cross(direction, Vector3.up).normalized;

            Gizmos.DrawLine(segment.Start + perpendicular, segment.End + perpendicular);
            Gizmos.DrawLine(segment.Start - perpendicular, segment.End - perpendicular);
        }
    }
}