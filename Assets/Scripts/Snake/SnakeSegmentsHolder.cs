using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeSegmentsHolder : MonoBehaviour
{
    private List<SnakeSegment> _segments = new List<SnakeSegment>();

    public void AddSegment(SnakeSegment segment)
    {
        _segments.Add(segment);
    }
}
