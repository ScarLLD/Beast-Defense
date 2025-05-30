using UnityEngine;
using System.Collections.Generic;

public class BoundaryMarker : MonoBehaviour
{
    [SerializeField] private Game _game;
    [SerializeField] private GameObject _pointPrefab;
    [SerializeField] private Material _lineMeterial;

    private List<Vector3> _planePoints;
    private List<LineRenderer> _lines;
    private Camera _camera;

    private void Awake()
    {
        _planePoints = new List<Vector3>();
        _lines = new List<LineRenderer>();
        _camera = Camera.main;
    }

    private void OnEnable()
    {
        _game.Started += GenerateMarkers;
    }

    private void OnDisable()
    {
        _game.Started -= GenerateMarkers;
    }

    public float GetBoundFloatX(int lineNumber)
    {
        return _lines[lineNumber].transform.position.x;
    }


    public Vector3 GetRandomPointOnRandomLine()
    {
        int index = Random.Range(0, _lines.Count);
        var line = _lines[index];

        return GetRandomLinePoint(line.GetPosition(0), line.GetPosition(1));
    }

    private void GenerateMarkers()
    {
        float partHeight = _camera.pixelHeight * 3f / 4f;

        List<Vector3> screenPoints = new List<Vector3>()
        {
        new(0f, partHeight, 10),
        new(0f, _camera.pixelHeight, 10),
        new(_camera.pixelWidth, _camera.pixelHeight, 10),
        new(_camera.pixelWidth, partHeight, 10)
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
        GetRandomPointOnRandomLine();
    }

    private void CreateBorderLines()
    {
        if (_planePoints.Count < 2) return;

        for (int i = 0; i < _planePoints.Count - 1; i++)
        {
            int nextIndex = (i + 1) % _planePoints.Count;
            CreateLine(_planePoints[i], _planePoints[nextIndex]);
        }
    }

    private void CreateLine(Vector3 start, Vector3 end)
    {
        GameObject line = new("BorderLine");
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
        float randomT = Random.Range(0.1f, 0.9f);
        Vector3 randomPosition = Vector3.Lerp(start, end, randomT);
        Instantiate(_pointPrefab, randomPosition, Quaternion.identity, transform);

        return randomPosition;
    }
}