using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AvailabilityManagement : MonoBehaviour
{
    [SerializeField] private GridStorage _gridStorage;
    [SerializeField] private CubeCreator _cubeCreator;
    [SerializeField] private GridCreator _gridCreator;

    private List<GridCell>[,] _cells;

    private void OnEnable()
    {
        _cubeCreator.Created += OnCubesCreated;
    }

    private void OnDisable()
    {
        _cubeCreator.Created -= OnCubesCreated;
    }

    private void OnCubesCreated()
    {
        _cells = new List<GridCell>[_gridCreator.Rows, _gridCreator.Columns];

        for (int i = 0; i < _cells.GetLength(0); i++)
        {
            for (int j = 0; j < _cells.GetLength(1); j++)
            {
                _cells[i, j] = new List<GridCell>();

                int index = i * _gridCreator.Columns + j;

                if (index < _gridStorage.GridCount && _gridStorage.TryGet(index, out GridCell cell))
                {
                    _cells[i, j].Add(cell);
                }
            }
        }

        UpdateAvailability();
    }

    public void UpdateAvailability()
    {
        for (int i = 0; i < _cells.GetLength(0); i++)
        {
            for (int j = 0; j < _cells.GetLength(1); j++)
            {
                for (int k = 0; k < _cells[i, j].Count; k++)
                {
                    var cell = _cells[i, j][k];

                    bool isTopRow = i == _cells.GetLength(0) - 1;
                    bool isLeftEdge = j == 0;
                    bool isRightEdge = j == _cells.GetLength(1) - 1;
                    bool isBottomEdge = i == 0;

                    bool haveStaticLeft = isLeftEdge == false && _cells[i, j - 1].Any(cell => cell.Cube.IsStatic);
                    bool haveStaticRight = isRightEdge == false && _cells[i, j + 1].Any(cell => cell.Cube.IsStatic);
                    bool haveStaticBottom = isBottomEdge == false && _cells[i - 1, j].Any(cell => cell.Cube.IsStatic);
                    bool haveStaticTop = isTopRow == false && _cells[i + 1, j].Any(cell => cell.Cube.IsStatic);

                    if (cell.Cube.IsStatic)
                    {
                        bool isAvailable = false;

                        if (isTopRow)
                        {
                            cell.SetIsTopRow(true);
                            cell.Cube.ChangeAvailableStatus(true);
                            continue;
                        }

                        if (isBottomEdge)
                        {
                            if (isLeftEdge && (haveStaticTop == false || haveStaticRight == false))
                                isAvailable = true;
                            else if (isRightEdge && (haveStaticTop == false || haveStaticLeft == false))
                                isAvailable = true;
                            else if (isLeftEdge == false && isRightEdge == false && (haveStaticTop == false || haveStaticLeft == false || haveStaticRight == false))
                                isAvailable = true;
                        }
                        else if (isLeftEdge)
                        {
                            if (isTopRow && (haveStaticBottom == false || haveStaticRight == false))
                                isAvailable = true;
                            else if (isBottomEdge && (haveStaticTop == false || haveStaticRight == false))
                                isAvailable = true;
                            else if (isBottomEdge == false && isTopRow == false && (haveStaticTop == false || haveStaticRight == false || haveStaticBottom == false))
                                isAvailable = true;
                        }
                        else if (isRightEdge)
                        {
                            if (isTopRow && (haveStaticBottom == false || haveStaticLeft == false))
                                isAvailable = true;
                            else if (isBottomEdge && (haveStaticTop == false || haveStaticLeft == false))
                                isAvailable = true;
                            else if (isBottomEdge == false && isTopRow == false && (haveStaticTop == false || haveStaticLeft == false || haveStaticBottom == false))
                                isAvailable = true;
                        }
                        else if (!isLeftEdge && !isRightEdge && !isBottomEdge && (!haveStaticLeft || !haveStaticRight || !haveStaticBottom || !haveStaticTop))
                        {
                            isAvailable = true;
                        }

                        cell.Cube.ChangeAvailableStatus(isAvailable);
                    }
                    else
                    {
                        if (isTopRow == false && k < _cells[i + 1, j].Count)
                        {
                            var topCell = _cells[i + 1, j][k];
                            topCell.TakeCell(cell);
                        }

                        if (isLeftEdge == false && k < _cells[i, j - 1].Count)
                        {
                            var leftCell = _cells[i, j - 1][k];
                            leftCell.TakeCell(cell);
                        }

                        if (isRightEdge == false && k < _cells[i, j + 1].Count)
                        {
                            var rightCell = _cells[i, j + 1][k];
                            rightCell.TakeCell(cell);
                        }

                        if (isBottomEdge == false && k < _cells[i - 1, j].Count)
                        {
                            var bottomCell = _cells[i - 1, j][k];
                            bottomCell.TakeCell(cell);
                        }
                    }
                }
            }
        }
    }
}