using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.MapGenerator.Grid
{
    public class GridStorage : MonoBehaviour
    {
        private List<GridCell> _grid;
        private List<GridCell>[,] _cells;

        public IReadOnlyList<GridCell>[,] Cells => _cells;

        public int GridCount => _grid.Count;
        public List<GridCell> GetAllCells => _grid;

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

            return gridCell;
        }

        public void CreateCells(int rows, int columns)
        {
            _cells = new List<GridCell>[rows, columns];

            for (var i = 0; i < _cells.GetLength(0); i++)
            {
                for (var j = 0; j < _cells.GetLength(1); j++)
                {
                    _cells[i, j] = new List<GridCell>();

                    var index = i * columns + j;

                    if (index < _grid.Count && TryGet(index, out var cell))
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
}