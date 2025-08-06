using System.Collections.Generic;
using UnityEngine;

public class GridCell : MonoBehaviour
{
    public PlayerCube Cube;

    private List<GridCell> _availableCells;

    public IReadOnlyList<GridCell> AvailableCells => _availableCells;

    private void Awake()
    {
        _availableCells = new List<GridCell>();
    }

    public bool IsTopRow { get; private set; } = false;

    public void TakeCell(GridCell cell)
    {
        _availableCells.Add(cell);
    }

    public void SetIsTopRow(bool isTopRow)
    {
        IsTopRow = isTopRow;
    }

    public void TakeCube(PlayerCube cube)
    {
        Cube = cube;
    }
}
