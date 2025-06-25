using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(ColorsHolder), typeof(CubesCounterHolder))]
public class SpecificCubesCreator : MonoBehaviour
{
    [SerializeField] private int _count;
    [SerializeField] private PathHolder _pathHolder;
    [SerializeField] private GridAndCubesGenerator _generator;

    public IReadOnlyList<CustomCube> Cubes => _cubes;

    private List<CustomCube> _cubes = new List<CustomCube>();

    private ColorsHolder _colorsHolder;
    private CubesCounterHolder _cubesCounterHolder;

    public event Action Created;

    private void Awake()
    {
        _colorsHolder = GetComponent<ColorsHolder>();
        _cubesCounterHolder = GetComponent<CubesCounterHolder>();
    }

    private void OnEnable()
    {
        _pathHolder.PathInit += CreateCubes;
    }

    private void OnDisable()
    {
        _pathHolder.PathInit -= CreateCubes;
    }

    public void CreateCubes()
    {
        if (_generator.TryGetGridPositions(out List<Vector3> positions))
        {
            for (int i = 0; i < positions.Count; i++)
            {
                _cubes.Add(new CustomCube(_cubesCounterHolder.GetRandomCount(), _colorsHolder.GetRandomColor()));
            }

            _generator.SpawnCubes(_cubes);
            Created?.Invoke();

            Debug.Log($"Кубики созданы: {_cubes.Count}");
        }
    }
}
