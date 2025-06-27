using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeStorage : MonoBehaviour
{
    private List<CubeStack> _stacks = new List<CubeStack>();

    public void Add(CubeStack stack)
    {
        _stacks.Add(stack);
    }
}
