using UnityEngine;

[System.Serializable]
public class SegmentData
{
    public SnakeSegment Segment;
    public float DistanceAlongSpline;
    public Vector3 CurrentPosition;
    public Quaternion CurrentRotation;
}
