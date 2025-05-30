using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionAnalyzer : MonoBehaviour
{
    [SerializeField] private BoundaryMarker _marker;

    private float _leftBoundX;

    private void Awake()
    {
        _leftBoundX = _marker.Lines[0].X
    }
}
