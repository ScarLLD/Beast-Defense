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
        Debug.Log(_segments.Count());
    }

    public void RemoveTarget(SnakeSegment segment)
    {
        _segments.Remove(segment);
    }

    public bool TryGetSegment(Material material, out SnakeSegment segment)
    {
        segment = _segments.FirstOrDefault(segment => segment.enabled);
        return segment != null;
    }
}
