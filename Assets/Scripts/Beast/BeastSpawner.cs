using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class BeastSpawner : MonoBehaviour
{
    [SerializeField] private Beast _beastPrefab;

    public void Spawn( Snake snake, SplineContainer splineContainer)
    {
        var beast = Instantiate(_beastPrefab, transform);
        beast.Init(snake, splineContainer);
    }
}
