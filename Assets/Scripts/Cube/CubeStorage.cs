using System;
using System.Collections.Generic;
using UnityEngine;

public class CubeStorage : MonoBehaviour
{
    [SerializeField] private int _cubesPerSegment = 4;

    private List<ICube> _stacks = new List<ICube>();

    public IReadOnlyList<ICube> Stacks => _stacks;

    public Material GetMaterial(int i)
    {
        return _stacks[i * _cubesPerSegment].Material;
    }

    public void Add(ICube cube)
    {
        _stacks.Add(cube);
    }
}
