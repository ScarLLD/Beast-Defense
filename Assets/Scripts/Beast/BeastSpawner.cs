using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class BeastSpawner : MonoBehaviour
{
    [SerializeField] private Beast _beastPrefab;

    private Beast _beast;
    private Transform _transform;

    private void Awake()
    {
        _transform = transform;
    }

    public void Spawn(Snake snake, SplineContainer splineContainer)
    {
        if (_beast == null)
            _beast = Instantiate(_beastPrefab, _transform);

        _beast.Init(snake, splineContainer);
    }
}
