using System.Collections.Generic;
using UnityEngine;

public class CubeStorage : MonoBehaviour
{
    private readonly List<ICube> _stacks = new();

    public IReadOnlyList<ICube> Stacks => _stacks;

    public void Add(ICube cube)
    {
        _stacks.Add(cube);
    }
}
