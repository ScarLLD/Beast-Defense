using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TargetsHolder : MonoBehaviour
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

    public bool TryGetSegment(out SnakeSegment segment)
    {
        return _segments.FirstOrDefault(segment => segment.)
    }
}
