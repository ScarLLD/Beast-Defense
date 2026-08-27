using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.MapGenerator.Grid
{
    public class GridStorage : MonoBehaviour
    {
        private List<GridCell>[,] _cells;

        public IReadOnlyList<GridCell>[,] Cells => _cells;

        public int GridCount => GetAllCells.Count;
        public List<GridCell> GetAllCells { get; private set; }

        private void Awake()
        {
            GetAllCells = new List<GridCell>();
        }

        public void Add(GridCell gridCell)
        {
            GetAllCells.Add(gridCell);
        }

        public bool TryGet(int index, out GridCell gridCell)
        {
            gridCell = null;

            if (GetAllCells.Count >= index)
                gridCell = GetAllCells[index];

            return gridCell;
        }

        public void CreateCells(int rows, int columns)
        {
            _cells = new List<GridCell>[rows, columns];

            for (var i = 0; i < _cells.GetLength(0); i++)
            for (var j = 0; j < _cells.GetLength(1); j++)
            {
                _cells[i, j] = new List<GridCell>();

                var index = i * columns + j;

                if (index < GetAllCells.Count && TryGet(index, out var cell)) _cells[i, j].Add(cell);
            }
        }

        public void Clear()
        {
            foreach (var cell in GetAllCells) Destroy(cell.gameObject);

            GetAllCells.Clear();
        }
    }
}