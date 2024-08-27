using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineController : MonoBehaviour
{
    public LineRenderer lineRenderer; // Reference to the Line Renderer for the path

    void Start()
    {
        // Ensure we have a reference to the LineRenderer
        if (lineRenderer == null)
        {
            lineRenderer = GetComponent<LineRenderer>();
        }

        // Initialize the Line Renderer with no positions
        lineRenderer.positionCount = 0;
    }

    // Method to extend the LineRenderer when a waypoint is hit
    public void ExtendLineRenderer(Vector3 waypointPosition)
    {
        // Add a new position to the LineRenderer
        int newPositionIndex = lineRenderer.positionCount;
        lineRenderer.positionCount = newPositionIndex + 1;
        lineRenderer.SetPosition(newPositionIndex, waypointPosition);

        Debug.Log("LineRenderer extended to: " + waypointPosition);
    }
}