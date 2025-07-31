using System.Collections.Generic;
using UnityEngine;

public class GridStorage : MonoBehaviour
{
    private List<GridCell> _grid;

    public int GridCount => _grid.Count;

    private void Awake()
    {
        _grid = new List<GridCell>();
    }

    public void Add(GridCell gridCell)
    {
        _grid.Add(gridCell);
    }

    public bool TryGet(int index, out GridCell gridCell)
    {
        gridCell = null;

        if (_grid.Count >= index)
            gridCell = _grid[index];

        return gridCell != null;
    }
}
