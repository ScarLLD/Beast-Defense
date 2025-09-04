using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TargetStorage : MonoBehaviour
{
    private List<SnakeSegment> _segments;

    private void Awake()
    {
        _segments = new List<SnakeSegment>();
    }

    public int Count => _segments.Count;

    public void AddTarget(SnakeSegment segment)
    {
        _segments.Add(segment);
    }

    public bool TryGetTarget(Color color, out SnakeSegment snakeSegment)
    {
        snakeSegment = _segments.FirstOrDefault(segment => segment.IsCurrectColor(color) && segment.IsTarget == false);

        if (snakeSegment != null)
        {
            snakeSegment.SetIsTarget(true);
            return true;
        }

        return false;
    }
}
