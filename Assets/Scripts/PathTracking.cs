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
            TrackWaypoints();
        }
    }

    void TrackWaypoints()
    {
        if (Vector3.Distance(transform.position, waypoints[currentWaypointIndex].position) < 0.1f)
        {
            currentWaypointIndex++;
            if (currentWaypointIndex >= waypoints.Length)
            {
                loopCompleted = true;
                ActivateLineRenderer();
            }
        }
    }

    void ActivateLineRenderer()
    {
        lineRenderer.enabled = true;
        lineRenderer.positionCount = waypoints.Length;

        for (int i = 0; i < waypoints.Length; i++)
        {
            lineRenderer.SetPosition(i, waypoints[i].position);
        }

        // Optionally close the loop
        lineRenderer.loop = true;
    }
}
