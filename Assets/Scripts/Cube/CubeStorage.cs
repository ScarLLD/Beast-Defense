using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;

public class CubeStorage : MonoBehaviour
{
    private List<ICube> _stacks = new List<ICube>();

    public IReadOnlyList<ICube> Stacks => _stacks;

    public void Add(ICube cube)
    {
        _stacks.Add(cube);
    }
}
