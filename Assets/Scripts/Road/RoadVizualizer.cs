using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RoadSpawner))]
public class RoadVizualizer : MonoBehaviour
{
    [SerializeField] private GameObject _pathRoadPrefab;
    [SerializeField] private GameObject _pathCornerPrefab;
    [SerializeField] private float _prefabScale;
    [SerializeField] private float _prefabScaleYmultiplier;

    private readonly float _divider = 2f;

    public void VisualizeRoad(List<Vector3> road)
    {
        foreach (Transform child in transform)
            Destroy(child.gameObject);

        for (int i = 0; i < road.Count - 1; i++)
        {
            CreatePathSegment(road, i);
            CreateRoadPoint(road, i);
        }
    }

    private void CreateRoadPoint(List<Vector3> pathPoints, int index)
    {
        var point = Instantiate(_pathCornerPrefab, transform);
        point.transform.position = pathPoints[index + 1];
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
