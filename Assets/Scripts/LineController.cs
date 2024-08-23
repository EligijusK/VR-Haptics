using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineController : MonoBehaviour
{
    private LineRenderer lineRenderer;
    [SerializeField] private Transform[] points;

    private int currentPointIndex = 0; // Index to track the current waypoint

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0; // Start with no points
    }

    void Update()
    {
        if (currentPointIndex < points.Length)
        {
            CheckWaypointReached(); // Check if the current waypoint is reached
        }
    }

    void CheckWaypointReached()
    {
        // Check if the object is close enough to the current waypoint
        if (Vector3.Distance(transform.position, points[currentPointIndex].position) < 0.1f)
        {
            // Add the current waypoint to the Line Renderer
            lineRenderer.positionCount = currentPointIndex + 1;
            lineRenderer.SetPosition(currentPointIndex, points[currentPointIndex].position);

            // Move to the next waypoint
            currentPointIndex++;
        }
    }
}
