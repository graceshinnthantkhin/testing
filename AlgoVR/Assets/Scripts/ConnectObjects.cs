using UnityEngine;

public class ConnectObjects : MonoBehaviour
{
    public Transform objectA;
    public Transform objectB;
    private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        // Ensure the line has exactly two points
        lineRenderer.positionCount = 2;
    }

    void Update()
    {
        if (objectA != null && objectB != null)
        {
            // Set position 0 to A and position 1 to B
            lineRenderer.SetPosition(0, objectA.position);
            lineRenderer.SetPosition(1, objectB.position);
        }
    }
}
