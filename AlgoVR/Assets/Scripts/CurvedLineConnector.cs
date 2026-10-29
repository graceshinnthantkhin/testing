using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class CurvedLineConnector : MonoBehaviour
{
    public Transform objectA;
    public Transform objectB;
    public int resolution = 20; // Number of segments for the curve
    public float curveHeight = 0.2f; // Adjust to change arc steepness

    private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = resolution;
        lineRenderer.useWorldSpace = true; // Ensure world space for moving objects
    }

    void Update()
    {
        if (objectA != null && objectB != null)
        {
            DrawCurve();
        }
    }

    void DrawCurve()
    {
        // Calculate a control point in the middle, pushed upward to create the arc
        Vector3 startPos = objectA.position;
        Vector3 endPos = objectB.position;
        Vector3 midPoint = Vector3.Lerp(startPos, endPos, 0.5f);
        Vector3 controlPoint = midPoint + Vector3.up * curveHeight;

        for (int i = 0; i < resolution; i++)
        {
            float t = i / (float)(resolution - 1);
            // Quadratic Bezier Formula
            Vector3 point = CalculateBezierPoint(t, startPos, controlPoint, endPos);
            lineRenderer.SetPosition(i, point);
        }
    }

    Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        // Formula: (1-t)^2 * P0 + 2 * (1-t) * t * P1 + t^2 * P2
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        Vector3 p = uu * p0;
        p += 2 * u * t * p1;
        p += tt * p2;
        return p;
    }
}
