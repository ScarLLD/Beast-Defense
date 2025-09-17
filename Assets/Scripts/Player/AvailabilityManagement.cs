using System.Linq;
using UnityEngine;

public class AvailabilityManagement : MonoBehaviour
{
    [SerializeField] private GridStorage _gridStorage;

    public void UpdateAvailability()
    {
        var cells = _gridStorage.Cells;

        for (int i = 0; i < cells.GetLength(0); i++)
        {
            for (int j = 0; j < cells.GetLength(1); j++)
            {
                for (int k = 0; k < cells[i, j].Count; k++)
                {
                    var cell = cells[i, j][k];

                    bool isTopRow = i == cells.GetLength(0) - 1;
                    bool isLeftEdge = j == 0;
                    bool isRightEdge = j == cells.GetLength(1) - 1;
                    bool isBottomEdge = i == 0;

                    bool haveStaticLeft = isLeftEdge == false && cells[i, j - 1].Any(cell => cell.IsStatic);
                    bool haveStaticRight = isRightEdge == false && cells[i, j + 1].Any(cell => cell.IsStatic);
                    bool haveStaticBottom = isBottomEdge == false && cells[i - 1, j].Any(cell => cell.IsStatic);
                    bool haveStaticTop = isTopRow == false && cells[i + 1, j].Any(cell => cell.IsStatic);

                    if (cell.IsStatic && cell.IsOccupied == false)
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
                    else if (cell.IsOccupied == false)
                    {
                        if (isTopRow == false && k < cells[i + 1, j].Count)
                        {
                            var topCell = cells[i + 1, j][k];
                            topCell.TakeCell(cell);
                        }

                        if (isLeftEdge == false && k < cells[i, j - 1].Count)
                        {
                            var leftCell = cells[i, j - 1][k];
                            leftCell.TakeCell(cell);
                        }

                        if (isRightEdge == false && k < cells[i, j + 1].Count)
                        {
                            var rightCell = cells[i, j + 1][k];
                            rightCell.TakeCell(cell);
                        }

                        if (isBottomEdge == false && k < cells[i - 1, j].Count)
                        {
                            var bottomCell = cells[i - 1, j][k];
                            bottomCell.TakeCell(cell);
                        }
                    }
                }
            }
        }
    }
}