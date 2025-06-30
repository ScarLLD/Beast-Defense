using System.Collections.Generic;
using UnityEngine;

public class PathVizualizator : MonoBehaviour
{
    [SerializeField] private GameObject _pathRoadPrefab;
    [SerializeField] private GameObject _pathCornerPrefab;
    [SerializeField] private float _prefabScale;
    [SerializeField] private float _prefabScaleYmultiplier;

    private readonly float _divider = 2f;

    public void VisualizePath(List<Vector3> pathPoints)
    {
        foreach (Transform child in transform)
            Destroy(child.gameObject);

        for (int i = 0; i < pathPoints.Count - 1; i++)
        {
            CreatePathSegment(pathPoints, i);
            CreatePathPoint(pathPoints, i);
        }
    }

    private void CreatePathPoint(List<Vector3> pathPoints, int i)
    {
        var point = Instantiate(_pathCornerPrefab, transform);
        point.transform.position = pathPoints[i + 1];
        point.transform.localScale = new Vector3(_prefabScale, _prefabScale / _divider * _prefabScaleYmultiplier, _prefabScale);
    }

    private void CreatePathSegment(List<Vector3> pathPoints, int index)
    {
        var segment = Instantiate(_pathRoadPrefab, transform);
        segment.transform.position = (pathPoints[index] + pathPoints[index + 1]) / _divider;
        segment.transform.LookAt(pathPoints[index + 1]);
        segment.transform.localScale = new Vector3(_prefabScale, _prefabScale * _prefabScaleYmultiplier, Vector3.Distance(pathPoints[index], pathPoints[index + 1]));
    }
}
