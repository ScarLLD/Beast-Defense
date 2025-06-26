using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ColorsHolder), typeof(CubesCounterHolder))]
public class CustomCubesCreator : MonoBehaviour
{
    [SerializeField] private int _count;
    [SerializeField] private PathHolder _pathHolder;
    [SerializeField] private GridAndCubesGenerator _gridGenerator;

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
        if (_gridGenerator.TryGetGridPositions(out List<Vector3> positions))
        {
            for (int i = 0; i < positions.Count; i++)
            {
                _cubes.Add(new CustomCube(_cubesCounterHolder.GetRandomCount(), _colorsHolder.GetRandomMaterial()));
            }

            _gridGenerator.SpawnCubes(_cubes);
            Created?.Invoke();

            Debug.Log($"Кубики созданы: {_cubes.Count}");
        }
    }
}
