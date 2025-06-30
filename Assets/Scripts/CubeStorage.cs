using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;

public class CubeStorage : MonoBehaviour
{
    private List<CubeStack> _stacks = new List<CubeStack>();

    public IReadOnlyList<CubeStack> Stacks => _stacks;

    public void Add(CubeStack stack)
    {
        _stacks.Add(stack);
    }
}
