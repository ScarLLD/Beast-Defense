using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;

public class SplineCreator : MonoBehaviour
{
    [SerializeField] private float _tangentLength = 3f;
    [SerializeField] private float _smoothness = 0.5f;
    [SerializeField] private int _subdivisions = 3;
    [SerializeField] private bool _useBezierSmoothing = true;
    [SerializeField] private bool _isBezier = true;

    public bool TryCreateSpline(List<Vector3> roadPoints, out SplineContainer splineContainer)
    {
        splineContainer = null;

        if (roadPoints == null || roadPoints.Count < 2)
            return false;

        GameObject splineObject = new("DynamicSpline");
        splineContainer = splineObject.AddComponent<SplineContainer>();
        splineObject.transform.position = Vector3.zero;

        Spline spline = splineContainer.Spline;
        spline.Clear();

        List<Vector3> processedPoints = null;

        if (_useBezierSmoothing)
        {
            if (_isBezier)
                processedPoints = SmoothPointsWithBezier(roadPoints);
            else
                processedPoints = SmoothPointsWithCatmullRom(roadPoints);
        }

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

                Vector3 bisector = (inDirection + outDirection).normalized;

                float angle = Vector3.Angle(inDirection, outDirection);
                float tangentMultiplier = Mathf.Lerp(0.3f, 1.5f, angle / 180f);

                knot.TangentIn = new float3(_smoothness * _tangentLength * tangentMultiplier * -bisector);
                knot.TangentOut = new float3(_smoothness * _tangentLength * tangentMultiplier * bisector);
            }
            else if (i == 0 && processedPoints.Count > 1)
            {
                Vector3 direction = (processedPoints[i + 1] - processedPoints[i]).normalized;
                knot.TangentOut = new float3(_smoothness * _tangentLength * direction);
            }
            else if (i == processedPoints.Count - 1 && processedPoints.Count > 1)
            {
                Vector3 direction = (processedPoints[i] - processedPoints[i - 1]).normalized;
                knot.TangentIn = new float3(_smoothness * _tangentLength * -direction);
            }

            spline.Add(knot);
        }

        spline.Closed = false;
        return true;
    }

    private List<Vector3> SmoothPointsWithBezier(List<Vector3> originalPoints)
    {
        if (originalPoints.Count < 3)
            return originalPoints;

        List<Vector3> smoothedPoints = new()
        {
            originalPoints[0]
        };

        for (int i = 0; i < originalPoints.Count - 1; i++)
        {
            Vector3 p0 = originalPoints[i];
            Vector3 p3 = originalPoints[i + 1];

            Vector3 p1, p2;

            if (i == 0)
            {
                Vector3 direction = (p3 - p0).normalized;
                p1 = p0 + _smoothness * _tangentLength * direction;
                p2 = p3 - _smoothness * _tangentLength * direction;
            }
            else if (i == originalPoints.Count - 2)
            {
                Vector3 direction = (p3 - p0).normalized;
                p1 = p0 + _smoothness * _tangentLength * direction;
                p2 = p3 - _smoothness * _tangentLength * direction;
            }
            else
            {
                Vector3 prevPoint = originalPoints[i - 1];
                Vector3 nextPoint = originalPoints[i + 2];

                Vector3 inDirection = (p0 - prevPoint).normalized;
                Vector3 outDirection = (nextPoint - p3).normalized;

                p1 = p0 + _smoothness * _tangentLength * inDirection;
                p2 = p3 - _smoothness * _tangentLength * outDirection;
            }

            for (int j = 1; j <= _subdivisions; j++)
            {
                float t = j / (float)(_subdivisions + 1);
                Vector3 bezierPoint = CalculateBezierPoint(t, p0, p1, p2, p3);
                smoothedPoints.Add(bezierPoint);
            }

            if (i < originalPoints.Count - 2)
            {
                smoothedPoints.Add(p3);
            }
        }

        smoothedPoints.Add(originalPoints[^1]);

        return smoothedPoints;
    }

    private Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;

        Vector3 point = uuu * p0;
        point += 3 * uu * t * p1;
        point += 3 * u * tt * p2;
        point += ttt * p3;

        return point;
    }

    private List<Vector3> SmoothPointsWithCatmullRom(List<Vector3> points)
    {
        if (points.Count < 4)
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