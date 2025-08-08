using System;
using System.Collections.Generic;
using UnityEngine;

public class GridCell : MonoBehaviour
{
    public PlayerCube Cube;

    private List<GridCell> _availableCells;

    public bool IsTopRow { get; private set; } = false;
    public IReadOnlyList<GridCell> AvailableCells => _availableCells;

    private void Awake()
    {
        _availableCells = new List<GridCell>();
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
}
