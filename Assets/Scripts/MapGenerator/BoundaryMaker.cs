using System;
using UnityEngine;
using System.Collections.Generic;

public class BoundaryMaker : MonoBehaviour
{
    [SerializeField] private Material _lineMeterial;

    [Range(0.6f, 0.9f)]
    [SerializeField] private float _borderZLowerReduction;

    [Range(0.92f, 0.98f)]
    [SerializeField] private float _borderZUpperReduction;

    private List<Vector3> _planePoints;
    private List<LineRenderer> _lines;
    private Camera _camera;

    public event Action<List<Vector3>> PointsInitialized;

    private void Awake()
    {
        _planePoints = new List<Vector3>();
        _lines = new List<LineRenderer>();
        _camera = Camera.main;
    }

    public Vector3 GetRandomPointOnRandomLine()
    {
        int index = UserUtils.GetIntRandomNumber(0, _lines.Count);
        var line = _lines[index];

        return GetRandomLinePoint(line.GetPosition(0), line.GetPosition(1));
    }

    public bool TryGetScreenBottomCenter(out Vector3 bottomScreenCenter)
    {
        bottomScreenCenter = Vector3.zero;

        Vector3 screenPoint = new(_camera.pixelWidth * 0.5f, _camera.pixelHeight * 0.5f * 0.5f, 0f);

        Ray ray = _camera.ScreenPointToRay(screenPoint);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            bottomScreenCenter = hit.point;
        }

        return bottomScreenCenter != Vector3.zero;
    }

    public bool TryGeneratePathMarkers()
    {
        float partHeight = _camera.pixelHeight * _borderZLowerReduction;

        List<Vector3> screenPoints = new()
        {
        new(0f, partHeight, 10),
        new(0f, _camera.pixelHeight * _borderZUpperReduction, 10),

        new(_camera.pixelWidth, partHeight, 10),
        new(_camera.pixelWidth, _camera.pixelHeight * _borderZUpperReduction, 10),

        new(0f, _camera.pixelHeight, 10),
        new(_camera.pixelWidth, _camera.pixelHeight, 10)
        };

        _planePoints.Clear();

        foreach (Vector3 screenPoint in screenPoints)
        {
            Ray ray = _camera.ScreenPointToRay(screenPoint);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                _planePoints.Add(hit.point);
            }
        }

        CreateBorderLines();
        PointsInitialized?.Invoke(_planePoints);

        if (_planePoints.Count == 0)
            Debug.Log("Не удалось сгенерировать грань карты.");

        return _planePoints.Count > 0;
    }

    private void CreateBorderLines()
    {
        if (_planePoints.Count < 2) return;

        _lines.Clear();

        for (int i = 0; i < _planePoints.Count - 1; i += 2)
        {
            CreateLine(_planePoints[i], _planePoints[i + 1]);
        }
    }

    private void CreateLine(Vector3 start, Vector3 end)
    {
        GameObject line = new("Line");
        line.transform.SetParent(transform);

        LineRenderer lineRenderer = line.AddComponent<LineRenderer>();
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);

        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.material = _lineMeterial;

        _lines.Add(lineRenderer);
    }

    private Vector3 GetRandomLinePoint(Vector3 start, Vector3 end)
    {
        float randomT = UserUtils.GetFloatRandomNumber(0.3f, 0.7f);
        Vector3 randomPosition = Vector3.Lerp(start, end, randomT);

        return randomPosition;
    }
}