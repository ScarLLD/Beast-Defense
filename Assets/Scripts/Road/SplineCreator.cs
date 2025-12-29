using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class SplineCreator : MonoBehaviour
{
    [SerializeField] private float _tangentLength = 0.3f;
    [SerializeField] private float _cornerRadius = 3f;
    [SerializeField] private float _cornerSmoothness = 0.75f;
    [SerializeField] private int _subdivisions = 3;
    [SerializeField] private float _minAngleForRounding = 15f;

    public bool TryCreateSpline(List<Vector3> roadPoints, out SplineContainer splineContainer)
    {
        splineContainer = null;

        if (roadPoints == null || roadPoints.Count < 2)
            return false;

        GameObject splineObject = new("Spline");
        splineObject.transform.position = Vector3.zero;
        splineObject.transform.parent = transform;

        splineContainer = splineObject.AddComponent<SplineContainer>();

        Spline spline = splineContainer.Spline;
        spline.Clear();

        List<int> cornerIndices = FindCorners(roadPoints);
        List<Vector3> roundedPoints = CreateRoundedCorners(roadPoints, cornerIndices);
        List<Vector3> processedPoints = SmoothPointsWithCatmullRom(roundedPoints);

        for (int i = 0; i < processedPoints.Count; i++)
        {
            BezierKnot knot = new(processedPoints[i]);

            if (i > 0 && i < processedPoints.Count - 1)
            {
                Vector3 prevPoint = processedPoints[i - 1];
                Vector3 currentPoint = processedPoints[i];
                Vector3 nextPoint = processedPoints[i + 1];

                Vector3 inDirection = (currentPoint - prevPoint).normalized;
                Vector3 outDirection = (nextPoint - currentPoint).normalized;

                bool isCornerPoint = IsCornerPoint(i, processedPoints, roadPoints, cornerIndices);

                if (isCornerPoint)
                {
                    float angle = Vector3.Angle(inDirection, outDirection);

                    Vector3 tangentDirection = (inDirection + outDirection).normalized;
                    float tangentStrength = _cornerRadius * _cornerSmoothness * Mathf.Lerp(0.1f, 0.5f, angle / 90f);

                    knot.TangentIn = new float3(tangentStrength * -tangentDirection);
                    knot.TangentOut = new float3(tangentStrength * tangentDirection);
                }
                else
                {
                    Vector3 direction = (nextPoint - prevPoint).normalized;
                    float straightMultiplier = Mathf.Lerp(0.5f, 1.5f, _cornerSmoothness);
                    knot.TangentIn = new float3(_tangentLength * straightMultiplier * -direction);
                    knot.TangentOut = new float3(_tangentLength * straightMultiplier * direction);
                }
            }
            else if (i == 0 && processedPoints.Count > 1)
            {
                Vector3 direction = (processedPoints[i + 1] - processedPoints[i]).normalized;
                knot.TangentOut = new float3(_tangentLength * direction);
            }
            else if (i == processedPoints.Count - 1 && processedPoints.Count > 1)
            {
                Vector3 direction = (processedPoints[i] - processedPoints[i - 1]).normalized;
                knot.TangentIn = new float3(_tangentLength * -direction);
            }

            spline.Add(knot);
        }

        spline.Closed = false;
        return true;
    }

    private List<int> FindCorners(List<Vector3> points)
    {
        List<int> corners = new();

        for (int i = 1; i < points.Count - 1; i++)
        {
            Vector3 prevDir = (points[i] - points[i - 1]).normalized;
            Vector3 nextDir = (points[i + 1] - points[i]).normalized;

            float angle = Vector3.Angle(prevDir, nextDir);

            if (angle > _minAngleForRounding)
            {
                corners.Add(i);
            }
        }

        return corners;
    }

    private List<Vector3> CreateRoundedCorners(List<Vector3> originalPoints, List<int> cornerIndices)
    {
        if (cornerIndices.Count == 0)
            return originalPoints;

        List<Vector3> result = new();

        for (int i = 0; i < originalPoints.Count; i++)
        {
            if (!cornerIndices.Contains(i))
            {
                result.Add(originalPoints[i]);
            }
            else
            {
                Vector3 prevPoint = originalPoints[i - 1];
                Vector3 cornerPoint = originalPoints[i];
                Vector3 nextPoint = originalPoints[i + 1];

                Vector3 inDir = (cornerPoint - prevPoint).normalized;
                Vector3 outDir = (nextPoint - cornerPoint).normalized;

                float radius = _cornerRadius;
                Vector3 startPoint = cornerPoint - inDir * radius;
                Vector3 endPoint = cornerPoint + outDir * radius;

                if (result.Count == 0 || Vector3.Distance(result[^1], startPoint) > 0.01f)
                {
                    result.Add(startPoint);
                }

                for (int j = 1; j <= 3; j++)
                {
                    float t = j / 4f;

                    Vector3 point1 = Vector3.Lerp(startPoint, cornerPoint, t);
                    Vector3 point2 = Vector3.Lerp(cornerPoint, endPoint, t);
                    Vector3 smoothedPoint = Vector3.Lerp(point1, point2, t);

                    result.Add(smoothedPoint);
                }

                result.Add(endPoint);
            }
        }

        return result;
    }

    private bool IsCornerPoint(int index, List<Vector3> processedPoints, List<Vector3> originalPoints, List<int> cornerIndices)
    {
        foreach (int cornerIndex in cornerIndices)
        {
            float minDistance = float.MaxValue;
            int closestOriginalIndex = -1;

            for (int i = 0; i < originalPoints.Count; i++)
            {
                float dist = Vector3.Distance(processedPoints[index], originalPoints[i]);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    closestOriginalIndex = i;
                }
            }

            if (closestOriginalIndex >= 0 && Mathf.Abs(closestOriginalIndex - cornerIndex) <= 2)
            {
                return true;
            }
        }

        return false;
    }

    private List<Vector3> SmoothPointsWithCatmullRom(List<Vector3> points)
    {
        if (points.Count < 4 || _subdivisions <= 0)
            return points;

        List<Vector3> smoothed = new()
        {
            points[0]
        };

        for (int i = 0; i < points.Count - 1; i++)
        {
            Vector3 p0 = (i > 0) ? points[i - 1] : points[i];
            Vector3 p1 = points[i];
            Vector3 p2 = points[i + 1];
            Vector3 p3 = (i < points.Count - 2) ? points[i + 2] : points[i + 1];

            float segmentLength = Vector3.Distance(p1, p2);
            if (segmentLength < _cornerRadius * 0.5f)
            {
                smoothed.Add(p2);
                continue;
            }

            for (int j = 1; j <= _subdivisions; j++)
            {
                float t = j / (float)(_subdivisions + 1);
                Vector3 point = CalculateCatmullRomPoint(t, p0, p1, p2, p3);
                smoothed.Add(point);
            }

            smoothed.Add(p2);
        }

        return smoothed;
    }

    private Vector3 CalculateCatmullRomPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float t2 = t * t;
        float t3 = t2 * t;

        return 0.5f * ((2 * p1) +
                      (-p0 + p2) * t +
                      (2 * p0 - 5 * p1 + 4 * p2 - p3) * t2 +
                      (-p0 + 3 * p1 - 3 * p2 + p3) * t3);
    }
}