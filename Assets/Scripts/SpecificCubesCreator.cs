using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ColorsHolder), typeof(CubesCounterHolder))]
public class SpecificCubesCreator : MonoBehaviour
{
    private List<CustomCube> _cubes = new List<CustomCube>();

    private ColorsHolder _colorsHolder;
    private CubesCounterHolder _cubesCounterHolder;

    public event Action<List<CustomCube>> Created;

    private void Awake()
    {
        _colorsHolder = GetComponent<ColorsHolder>();
        _cubesCounterHolder = GetComponent<CubesCounterHolder>();
    }

    public void CreateCubes(int totalCubesCount)
    {
        for (int i = 0; i < totalCubesCount; i++)
        {
            _cubes.Add(new CustomCube(_cubesCounterHolder.GetRandomCount(), _colorsHolder.GetRandomColor()));
        }

        Created?.Invoke(_cubes);
    }
}
