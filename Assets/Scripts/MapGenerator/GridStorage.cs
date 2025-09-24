using System.Collections.Generic;
using UnityEngine;

public class GridStorage : MonoBehaviour
{
    private List<GridCell> _grid;
    private List<GridCell>[,] _cells;

    public IReadOnlyList<GridCell>[,] Cells => _cells;

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

    public void CreateCells(int rows, int columns)
    {
        _cells = new List<GridCell>[rows, columns];

        for (int i = 0; i < _cells.GetLength(0); i++)
        {
            for (int j = 0; j < _cells.GetLength(1); j++)
            {
                _cells[i, j] = new List<GridCell>();

                int index = i * columns + j;

                if (index < _grid.Count && TryGet(index, out GridCell cell))
                {
                    _cells[i, j].Add(cell);
                }
            }
        }
    }

    public void Clear()
    {
        foreach (var cell in _grid)
        {
            Destroy(cell.gameObject);
        }

        _grid.Clear();
    }
}
