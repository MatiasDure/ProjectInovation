using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
public class Spline : MonoBehaviour
{
    [HideInInspector]
    public List<Vector3> points = new List<Vector3>();

    public int ControlPointCount => points.Count;


    public void ResetSpline()
    {
        points = new List<Vector3>
        {
            Vector3.zero,
            Vector3.right - (Vector3.up/2),
            Vector3.right * 2 + (Vector3.up/2),
            Vector3.right * 3
        };
    }

    public Vector3 GetControlPoint(int index)
    {
        return transform.TransformPoint(points[index]);
    }

    public void SetControlPoint(int index, Vector3 point)
    {
        if (index >= points.Count || index < 0) return;
        points[index] = transform.InverseTransformPoint(point);
    }

    public void AddControlPoint(Vector3 point)
    {
        points.Add(transform.InverseTransformPoint(point));
    }
    public void RemoveLastPoint()
    {
        if(ControlPointCount >= 7)
        {
            int until = ControlPointCount - 3; 
            for (int i = ControlPointCount-1; i >= until; i--)
            {
                points.Remove(points[i]);
            }
        }
    }


    public Vector3 FindClosestPointOnSpline(Vector3 inputPoint)
    {
        float closestDistance = float.MaxValue;
        Vector3 closestPoint = Vector3.zero;
        int segments = ControlPointCount - 3;
        float step = 0.01f;

        for (int i = 0; i < segments; i += 3)
        {
            for (float t = 0; t <= 1f; t += step)
            {
                Vector3 p0 = GetControlPoint(i);
                Vector3 p1 = GetControlPoint(i + 1);
                Vector3 p2 = GetControlPoint(i + 2);
                Vector3 p3 = GetControlPoint(i + 3);
                Vector3 pointOnCurve = Bezier.GetPoint(p0, p1, p2, p3, t);
                float distance = Vector3.Distance(inputPoint, pointOnCurve);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestPoint = pointOnCurve;
                }
            }
        }
        return closestPoint;
    }

    public float GetPercentageOfSpline(Vector3 inputPoint)
    {
        Vector3 closestPoint = FindClosestPointOnSpline(inputPoint);
        float totalSplineLength = GetTotalSplineLength();
        float lengthToPoint = GetLengthToPoint(closestPoint);
        return lengthToPoint / totalSplineLength;
    }

    private float GetTotalSplineLength()
    {
        float totalLength = 0f;
        for (int i = 0; i < ControlPointCount - 3; i += 3)
        {
            totalLength += GetSegmentLength(i);
        }
        return totalLength;
    }

    private float GetSegmentLength(int startIndex)
    {
        float segmentLength = 0f;
        Vector3 previousPoint = GetControlPoint(startIndex);
        int segments = 10;
        float step = 1f / segments;

        for (float t = 0; t <= 1f; t += step)
        {
            Vector3 p0 = GetControlPoint(startIndex);
            Vector3 p1 = GetControlPoint(startIndex + 1);
            Vector3 p2 = GetControlPoint(startIndex + 2);
            Vector3 p3 = GetControlPoint(startIndex + 3);
            Vector3 currentPoint = Bezier.GetPoint(p0, p1, p2, p3, t);
            segmentLength += Vector3.Distance(previousPoint, currentPoint);
            previousPoint = currentPoint;
        }
        return segmentLength;
    }

    private float GetLengthToPoint(Vector3 point)
    {
        float lengthToPoint = 0f;
        bool found = false;

        for (int i = 0; i < ControlPointCount - 3 && !found; i += 3)
        {
            float segmentLength = GetSegmentLength(i);
            float previousLength = lengthToPoint;
            lengthToPoint += segmentLength;

            Vector3 p0 = GetControlPoint(i);
            Vector3 p1 = GetControlPoint(i + 1);
            Vector3 p2 = GetControlPoint(i + 2);
            Vector3 p3 = GetControlPoint(i + 3);

            for (float t = 0; t <= 1f; t += 0.01f)
            {
                Vector3 pointOnCurve = Bezier.GetPoint(p0, p1, p2, p3, t);
                float distance = Vector3.Distance(point, pointOnCurve);

                if (distance < 0.01f) // If the point is close enough to the curve
                {
                    found = true;
                    float lengthInSegment = GetLengthInSegment(p0, p1, p2, p3, point);
                    lengthToPoint = previousLength + lengthInSegment;
                    break;
                }
            }
        }

        return lengthToPoint;
    }
    private float GetLengthInSegment(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, Vector3 point)
    {
        float closestT = -1;
        float closestDistance = float.MaxValue;

        for (float t = 0; t <= 1f; t += 0.01f)
        {
            Vector3 pointOnCurve = Bezier.GetPoint(p0, p1, p2, p3, t);
            float distance = Vector3.Distance(point, pointOnCurve);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestT = t;
            }
        }

        float lengthInSegment = 0f;
        Vector3 previousPoint = p0;

        for (float t = 0; t <= closestT; t += 0.01f)
        {
            Vector3 currentPoint = Bezier.GetPoint(p0, p1, p2, p3, t);
            lengthInSegment += Vector3.Distance(previousPoint, currentPoint);
            previousPoint = currentPoint;
        }

        return lengthInSegment;
    }
}


public static class Bezier
{
    public static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        t = Mathf.Clamp01(t);
        float oneMinusT = 1f - t;
        return oneMinusT * oneMinusT * oneMinusT * p0 +
               3f * oneMinusT * oneMinusT * t * p1 +
               3f * oneMinusT * t * t * p2 +
               t * t * t * p3;
    }
}