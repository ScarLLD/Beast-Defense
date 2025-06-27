using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeSpawner : MonoBehaviour
{
    [SerializeField] private CubeStack _cubeStackPrefab;
    [SerializeField] private CubeStorage _cubeStorage;

    public void Spawn(Material material, int count)
    {
        CubeStack cube = Instantiate(_cubeStackPrefab);
        cube.Init()
        _cubeStorage.Add(cube);
    }
}
