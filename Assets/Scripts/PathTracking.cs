using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathTracking : MonoBehaviour
{
    public Transform[] waypoints; // Array of waypoints
    public LineRenderer lineRenderer; // Reference to the Line Renderer

    private int currentWaypointIndex = 0; // Index to track the current waypoint

    void Start()
    {
        // Ensure the Line Renderer is enabled
       // lineRenderer.enabled = true;
        // Initialize the Line Renderer with no positions
        lineRenderer.positionCount = 0;
    }

    void Update()
    {
        if (currentWaypointIndex < waypoints.Length)
        {
            TrackWaypoints(); // Continuously track the object's movement
        }
    }

    void TrackWaypoints()
    {
        // Check if the object is close enough to the current waypoint
        if (Vector3.Distance(transform.position, waypoints[currentWaypointIndex].position) < 0.1f)
        {
            // Add a new position to the Line Renderer
            lineRenderer.positionCount = currentWaypointIndex + 1;
            lineRenderer.SetPosition(currentWaypointIndex, waypoints[currentWaypointIndex].position);
            
            Debug.Log("Position added: " + waypoints[currentWaypointIndex].position); // Debug line
            // Move to the next waypoint
            currentWaypointIndex++;

            // If all waypoints are visited, finalize the loop (optional)
            if (currentWaypointIndex >= waypoints.Length)
            {
                FinalizeLineRenderer();
            }
        }
    }

    void FinalizeLineRenderer()
    {
        // Optionally close the loop
        lineRenderer.loop = true;
    }
}