using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace Game.Scripts.Road
{
    public class SplineCreator : MonoBehaviour
    {
        private const int POINTS_COUNT = 3;
        private const float DIVIDER = 4f;

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

            GameObject splineObject = new("Spline")
            {
                transform =
                {
                    position = Vector3.zero,
                    parent = transform
                }
            };

            splineContainer = splineObject.AddComponent<SplineContainer>();

            var spline = splineContainer.Spline;
            spline.Clear();

            var cornerIndices = FindCorners(roadPoints);
            var roundedPoints = CreateRoundedCorners(roadPoints, cornerIndices);
            var processedPoints = SmoothPointsWithCatmullRom(roundedPoints);

            for (var i = 0; i < processedPoints.Count; i++)
            {
                BezierKnot knot = new(processedPoints[i]);

                switch (i)
                {
                    case > 0 when i < processedPoints.Count - 1:
                    {
                        var prevPoint = processedPoints[i - 1];
                        var currentPoint = processedPoints[i];
                        var nextPoint = processedPoints[i + 1];

                        var inDirection = (currentPoint - prevPoint).normalized;
                        var outDirection = (nextPoint - currentPoint).normalized;

                        var isCornerPoint = IsCornerPoint(i, processedPoints, roadPoints, cornerIndices);

                        if (isCornerPoint)
                        {
                            var angle = Vector3.Angle(inDirection, outDirection);

                            var tangentDirection = (inDirection + outDirection).normalized;
                            var tangentStrength = _cornerRadius * _cornerSmoothness 
                                                                * Mathf.Lerp(0.1f, 0.5f, angle / 90f);

                            knot.TangentIn = new float3(tangentStrength * -tangentDirection);
                            knot.TangentOut = new float3(tangentStrength * tangentDirection);
                        }
                        else
                        {
                            var direction = (nextPoint - prevPoint).normalized;
                            var straightMultiplier = Mathf.Lerp(0.5f, 1.5f, _cornerSmoothness);
                            knot.TangentIn = new float3(_tangentLength * straightMultiplier * -direction);
                            knot.TangentOut = new float3(_tangentLength * straightMultiplier * direction);
                        }

                        break;
                    }
                    case 0 when processedPoints.Count > 1:
                    {
                        var direction = (processedPoints[i + 1] - processedPoints[i]).normalized;
                        knot.TangentOut = new float3(_tangentLength * direction);
                        break;
                    }
                    default:
                    {
                        if (i == processedPoints.Count - 1 && processedPoints.Count > 1)
                        {
                            var direction = (processedPoints[i] - processedPoints[i - 1]).normalized;
                            knot.TangentIn = new float3(_tangentLength * -direction);
                        }

                        break;
                    }
                }

                spline.Add(knot);
            }

            spline.Closed = false;
            return true;
        }
        
        private static Vector3 CalculateCatmullRomPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            var t2 = t * t;
            var t3 = t2 * t;

            return 0.5f * ((2 * p1) +
                           (-p0 + p2) * t +
                           (2 * p0 - 5 * p1 + 4 * p2 - p3) * t2 +
                           (-p0 + 3 * p1 - 3 * p2 + p3) * t3);
        }

        private List<int> FindCorners(List<Vector3> points)
        {
            List<int> corners = new();

            for (var i = 1; i < points.Count - 1; i++)
            {
                var prevDir = (points[i] - points[i - 1]).normalized;
                var nextDir = (points[i + 1] - points[i]).normalized;

                var angle = Vector3.Angle(prevDir, nextDir);

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

            for (var i = 0; i < originalPoints.Count; i++)
            {
                if (!cornerIndices.Contains(i))
                {
                    result.Add(originalPoints[i]);
                }
                else
                {
                    var prevPoint = originalPoints[i - 1];
                    var cornerPoint = originalPoints[i];
                    var nextPoint = originalPoints[i + 1];

                    var inDir = (cornerPoint - prevPoint).normalized;
                    var outDir = (nextPoint - cornerPoint).normalized;

                    var radius = _cornerRadius;
                    var startPoint = cornerPoint - inDir * radius;
                    var endPoint = cornerPoint + outDir * radius;

                    if (result.Count == 0 || Vector3.Distance(result[^1], startPoint) > 0.01f)
                    {
                        result.Add(startPoint);
                    }

                    for (var j = 1; j <= POINTS_COUNT; j++)
                    {
                        var t = j / DIVIDER;

                        var point1 = Vector3.Lerp(startPoint, cornerPoint, t);
                        var point2 = Vector3.Lerp(cornerPoint, endPoint, t);
                        var smoothedPoint = Vector3.Lerp(point1, point2, t);

                        result.Add(smoothedPoint);
                    }

                    result.Add(endPoint);
                }
            }

            return result;
        }

        private static bool IsCornerPoint(int index, List<Vector3> processedPoints, List<Vector3> originalPoints, List<int> cornerIndices)
        {
            foreach (var cornerIndex in cornerIndices)
            {
                var minDistance = float.MaxValue;
                var closestOriginalIndex = -1;

                for (var i = 0; i < originalPoints.Count; i++)
                {
                    var dist = Vector3.Distance(processedPoints[index], originalPoints[i]);
                    
                    if (!(dist < minDistance)) continue;
                    
                    minDistance = dist;
                    closestOriginalIndex = i;
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
                points[0],
            };

            for (var i = 0; i < points.Count - 1; i++)
            {
                var p0 = (i > 0) ? points[i - 1] : points[i];
                var p1 = points[i];
                var p2 = points[i + 1];
                var p3 = (i < points.Count - 2) ? points[i + 2] : points[i + 1];

                var segmentLength = Vector3.Distance(p1, p2);
                if (segmentLength < _cornerRadius * 0.5f)
                {
                    smoothed.Add(p2);
                    continue;
                }

                for (var j = 1; j <= _subdivisions; j++)
                {
                    var t = j / (float)(_subdivisions + 1);
                    var point = CalculateCatmullRomPoint(t, p0, p1, p2, p3);
                    smoothed.Add(point);
                }

                smoothed.Add(p2);
            }

            return smoothed;
        }
    }
}