using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoadFinder : MonoBehaviour
{
    public List<GridCell> FindShortestPath(GridCell startCell)
    {
        var visited = new HashSet<GridCell>();
        var queue = new Queue<List<GridCell>>();
        var initialPath = new List<GridCell> { startCell };
        queue.Enqueue(initialPath);

        while (queue.Count > 0)
        {
            var currentPath = queue.Dequeue();
            var lastCell = currentPath.Last();
                        
            if (lastCell.IsTopRow)
            {
                return currentPath;
            }
            
            foreach (var neighbor in lastCell.AvailableCells)
            {
                if (!visited.Contains(neighbor))
                {
                    visited.Add(neighbor);
                    var newPath = new List<GridCell>(currentPath) { neighbor };
                    queue.Enqueue(newPath);
                }
            }
        }

        return null;
    }

    public GridCell GetOptimalNextCell(GridCell currentCell)
    {
        var shortestPath = FindShortestPath(currentCell);
        return shortestPath?.Count > 1 ? shortestPath[1] : null;
    }
}
