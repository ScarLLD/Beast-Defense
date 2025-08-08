using System.Collections.Generic;
using UnityEngine;

public class CubeStorage : MonoBehaviour
{
    private readonly List<PlayerCube> _cubes = new();

    public void Add(PlayerCube cube)
    {
        _cubes.Add(cube);
    }

    public List<CubeStack> GetStacks()
    {
        List<CubeStack> cubeStacks = new();

        foreach (var cube in _cubes)
        {
            cubeStacks.Add(cube.GetStack);
        }

        return cubeStacks;
    }
}
