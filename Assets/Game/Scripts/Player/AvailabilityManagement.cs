using System.Linq;
using Game.Scripts.MapGenerator.Grid;

namespace Game.Scripts.Player
{
    public class AvailabilityManagement
    {
        private GridStorage _gridStorage;

        public void Init(GridStorage gridStorage)
        {
            _gridStorage = gridStorage;
        }

        public void UpdateAvailability()
        {
            var cells = _gridStorage.Cells;

            for (var i = 0; i < cells.GetLength(0); i++)
            {
                for (var j = 0; j < cells.GetLength(1); j++)
                {
                    for (var k = 0; k < cells[i, j].Count; k++)
                    {
                        var cell = cells[i, j][k];

                        var isTopRow = i == cells.GetLength(0) - 1;
                        var isLeftEdge = j == 0;
                        var isRightEdge = j == cells.GetLength(1) - 1;
                        var isBottomEdge = i == 0;

                        var haveStaticLeft = !isLeftEdge && cells[i, j - 1].Any(cell => cell.IsStatic);
                        var haveStaticRight = !isRightEdge && cells[i, j + 1].Any(cell => cell.IsStatic);
                        var haveStaticBottom = !isBottomEdge && cells[i - 1, j].Any(cell => cell.IsStatic);
                        var haveStaticTop = !isTopRow && cells[i + 1, j].Any(cell => cell.IsStatic);

                        if (cell.IsStatic && !cell.IsOccupied)
                        {
                            var isAvailable = false;

                            if (isTopRow)
                            {
                                cell.SetIsTopRow(true);
                                cell.Cube.ChangeAvailableStatus(true);
                                continue;
                            }

                            if (isBottomEdge)
                            {
                                if ((!isLeftEdge
                                     || (haveStaticTop && haveStaticRight))
                                    && (!isRightEdge || (haveStaticTop && haveStaticLeft))
                                    && (isLeftEdge || isRightEdge ||
                                        (haveStaticTop && haveStaticLeft && haveStaticRight)))
                                {
                                }
                                else
                                    isAvailable = true;
                            }
                            else if (isLeftEdge)
                            {
                                if ((!haveStaticTop || !haveStaticRight ||
                                     !haveStaticBottom))
                                    isAvailable = true;
                            }
                            else if (isRightEdge)
                            {
                                if (!haveStaticTop || !haveStaticLeft ||
                                    !haveStaticBottom)
                                    isAvailable = true;
                            }
                            else if (!haveStaticLeft || !haveStaticRight || !haveStaticBottom || !haveStaticTop)
                            {
                                isAvailable = true;
                            }

                            cell.Cube.ChangeAvailableStatus(isAvailable);
                        }
                        else if (!cell.IsOccupied)
                        {
                            if (!isTopRow && k < cells[i + 1, j].Count)
                            {
                                var topCell = cells[i + 1, j][k];
                                topCell.TakeCell(cell);
                            }

                            if (!isLeftEdge && k < cells[i, j - 1].Count)
                            {
                                var leftCell = cells[i, j - 1][k];
                                leftCell.TakeCell(cell);
                            }

                            if (!isRightEdge && k < cells[i, j + 1].Count)
                            {
                                var rightCell = cells[i, j + 1][k];
                                rightCell.TakeCell(cell);
                            }

                            if (isBottomEdge || k >= cells[i - 1, j].Count) continue;
                            
                            var bottomCell = cells[i - 1, j][k];
                            bottomCell.TakeCell(cell);
                        }
                    }
                }
            }
        }
    }
}