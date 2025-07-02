using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoadStorage : MonoBehaviour
{
    private List<Vector3> _pathPoints;

    public event Action Initialized;

    public void InitPoints(IReadOnlyList<Vector3> pathPoints)
    {
        _pathPoints = new List<Vector3>();

        foreach (var point in pathPoints)
        {
            _pathPoints.Add(point);
        }

        Initialized?.Invoke();
    }

    public bool TryGetStartPosition(out Vector3 spawnPoint)
    {
        spawnPoint = Vector3.zero;

        if(_pathPoints.Count > 0 )
            spawnPoint = _pathPoints.FirstOrDefault();

        return spawnPoint != Vector3.zero;
    }

    public bool TryGetNextPosition(Vector3 currentPosition, out Vector3 nextPosition)
    {
        nextPosition = Vector3.zero;

        if (_pathPoints.Contains(currentPosition))
        {
            int index = _pathPoints.IndexOf(currentPosition);

            if (_pathPoints.Count > index + 1)
            {
                nextPosition = _pathPoints[index + 1];
            }
        }

        return nextPosition != Vector3.zero;
    }
}
