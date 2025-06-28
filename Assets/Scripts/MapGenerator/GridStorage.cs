using System.Collections.Generic;
using UnityEngine;

public class GridStorage : MonoBehaviour
{
    private List<Vector3> _grid;

    public int GridCount => _grid.Count;

    private void Awake()
    {
        _grid = new List<Vector3>();
    }

    public void Add(Vector3 spawnPoint)
    {
        _grid.Add(spawnPoint);
    }

    public bool TryGet(int index, out Vector3 spawnPoint)
    {
        spawnPoint = Vector3.zero;

        if (_grid.Count >= index)
            spawnPoint = _grid[index];

        return spawnPoint != Vector3.zero;
    }
}
