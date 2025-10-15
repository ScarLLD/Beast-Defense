using System;
using System.Collections.Generic;
using UnityEngine;

public class GridCell : MonoBehaviour
{
    private List<GridCell> _availableCells;
    public PlayerCube Cube { get; private set; }
    public Obstacle Obstacle { get; private set; }

    public bool IsStatic { get; private set; }
    public bool IsTopRow { get; private set; }
    public bool IsOccupied => Obstacle != null;
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
        if (cell == null)
            throw new ArgumentNullException(nameof(cell), $"cell не может быть null.");

        _availableCells.Add(cell);
    }

    public void InitCube(PlayerCube cube)
    {
        if (cube == null)
            throw new ArgumentNullException(nameof(cube), $"cube не может быть null.");

        Cube = cube;
    }

    public void InitObstacle(Obstacle obstacle)
    {
        if (obstacle == null)
            throw new ArgumentNullException(nameof(obstacle), $"obstacle не может быть null.");

        Obstacle = obstacle;
    }
}