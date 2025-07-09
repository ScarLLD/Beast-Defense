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

    public void AddTarget(SnakeSegment segment)
    {
        _segments.Add(segment);
    }

    public void RemoveTarget(SnakeSegment segment)
    {
        _segments.Remove(segment);
    }

    public bool TryGetTarget(Color color, out SnakeSegment snakeSegment)
    {
        snakeSegment = _segments.FirstOrDefault(segment => segment.IsCurrectColor(color));

        if (snakeSegment != null)
            _segments.Remove(snakeSegment);

        return snakeSegment != null;
    }
}
