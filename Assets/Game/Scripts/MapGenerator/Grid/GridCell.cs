using Game.Scripts.Player;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.MapGenerator.Grid
{
    public class GridCell : MonoBehaviour
    {
        private Obstacle _obstacle;
        private List<GridCell> _availableCells;
        
        public PlayerCube Cube { get; private set; }
        public bool IsStatic { get; private set; }
        public bool IsTopRow { get; private set; }
        public bool IsOccupied => _obstacle;
        public IReadOnlyList<GridCell> AvailableCells => _availableCells;

        private void Awake()
        {
            _availableCells = new List<GridCell>();

            SetDefaultSettings();
        }

        public void SetDefaultSettings()
        {
            IsStatic = true;
            IsTopRow = false;

            _availableCells.Clear();
        }

        public void ChangeStaticStatus(bool isStatic)
        {
            IsStatic = isStatic;
        }

        public void SetIsTopRow(bool isTopRow)
        {
            IsTopRow = isTopRow;
        }

        public void TakeCell(GridCell cell)
        {
            if (!cell)
                throw new ArgumentNullException(nameof(cell), $"cell не может быть null.");

            _availableCells.Add(cell);
        }

        public void InitCube(PlayerCube cube)
        {
            if (!cube)
                throw new ArgumentNullException(nameof(cube), $"cube не может быть null.");

            Cube = cube;
        }

        public void InitObstacle(Obstacle obstacle)
        {
            if (!obstacle)
                throw new ArgumentNullException(nameof(obstacle), $"obstacle не может быть null.");

            _obstacle = obstacle;
        }
    }
}