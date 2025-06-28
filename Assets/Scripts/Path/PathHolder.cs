using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathHolder : MonoBehaviour
{
    [SerializeField] private SnakeHead _snakeHead;

    private List<Vector3> _pathPoints;

    public event Action PathInit;

    public void InitPoints(IReadOnlyList<Vector3> pathPoints)
    {
        _pathPoints = new List<Vector3>();

        foreach (var point in pathPoints)
        {
            _pathPoints.Add(new Vector3(point.x, point.y + _snakeHead.transform.localScale.y / 2, point.z));
        }

        PathInit?.Invoke();
    }

    public bool TryGetStartPosition(out Vector3 spawnPoint)
    {
        spawnPoint = _pathPoints.FirstOrDefault();
        return spawnPoint != null;
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
