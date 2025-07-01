using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeSegmentsStorage : MonoBehaviour
{
    private List<SnakeSegment> _segments = new List<SnakeSegment>();

    public void AddSegment(SnakeSegment segment)
    {
        _segments.Add(segment);
    }

    public bool TryGetSeveredSegments(SnakeSegment snakeSegment, out List<SnakeSegment> severedSegments)
    {
        severedSegments = new List<SnakeSegment>();

        if (_segments.Contains(snakeSegment))
        {
            int index = _segments.IndexOf(snakeSegment);

            for (int i = 0; i < index; i++)
            {
                severedSegments.Add(_segments[i]);
            }
        }

        return severedSegments.Count > 0;
    }
}
