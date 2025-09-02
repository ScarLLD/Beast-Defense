using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class SplineCreator : MonoBehaviour
{
    [SerializeField] private float _tangentLength = 3f;

    public bool TryCreateSplineWith90DegreeCorners(List<Vector3> roadPoints, out SplineContainer splineContainer)
    {
        splineContainer = null;

        if (roadPoints.Count == 0)
            return false;

        GameObject splineObject = new("DynamicSpline");
        splineContainer = splineObject.AddComponent<SplineContainer>();
        splineObject.transform.position = Vector3.zero;

        Spline spline = splineContainer.Spline;
        spline.Clear();

        for (int i = 0; i < roadPoints.Count; i++)
        {
            if (roadPoints[i] == null) continue;

            BezierKnot knot = new(roadPoints[i]);

            if (i > 0 && i < roadPoints.Count - 1)
            {
                Vector3 prevDir = (roadPoints[i] - roadPoints[i - 1]).normalized;
                Vector3 nextDir = (roadPoints[i + 1] - roadPoints[i]).normalized;

                Vector3 bisector = (prevDir + nextDir).normalized;

                knot.TangentIn = new float3(-bisector * _tangentLength);
                knot.TangentOut = new float3(bisector * _tangentLength);
            }

            spline.Add(knot);
        }

        spline.Closed = false;
        return true;
    }
}
