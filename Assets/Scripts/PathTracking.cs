using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathTracking : MonoBehaviour
{
    public Transform[] waypoints;
    public LineRenderer lineRenderer;

    private int currentWaypointIndex = 0;
    private bool loopCompleted = false;

    void Update()
    {
        if (!loopCompleted)
        {
            TrackWaypoints(); // Continuously track the object's movement
        }
    }

    void TrackWaypoints()
    {
        // Check if the object is close enough to the current waypoint
        if (Vector3.Distance(transform.position, waypoints[currentWaypointIndex].position) < 0.1f)
        {
            // Move to the next waypoint
            currentWaypointIndex++;

            // If all waypoints are visited, mark the loop as completed
            if (currentWaypointIndex >= waypoints.Length)
            {
                loopCompleted = true;
                ActivateLineRenderer();
            }
        }
    }

    void ActivateLineRenderer()
    {
        // Enable the Line Renderer
        lineRenderer.enabled = true;

        // Set the position count to the number of waypoints
        lineRenderer.positionCount = waypoints.Length;

        // Loop through each waypoint and set the corresponding position in the Line Renderer
        for (int i = 0; i < waypoints.Length; i++)
        {
            lineRenderer.SetPosition(i, waypoints[i].position);
        }

        // Optionally close the loop (if you want the line to connect back to the start)
        lineRenderer.loop = true;
    }
}
