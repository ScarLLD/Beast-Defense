using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathHolder : MonoBehaviour
{
    private List<Vector3> _pathPoints;

    public void InitPoints(IReadOnlyList<Vector3> pathPoints)
    {
        _pathPoints = pathPoints.ToList();
    }
}
