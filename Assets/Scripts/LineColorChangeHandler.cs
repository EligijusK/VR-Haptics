using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineColorChangeHandler : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public Material collisionMaterial;  // Assign the material you want to change to upon collision
    public Material defaultMaterial;    // Assign the default material

    public LineController lineController;

    void Start()
    {
        // Ensure we have a reference to LineController
        if (lineController == null)
        {
            lineController = FindObjectOfType<LineController>();
        }
    }

    // void OnCollisionEnter(Collision collision)
    // {
    //     // Ensure collision only happens on the correct sequential waypoint
    //     if (lineController.currentPointIndex >= lineRenderer.positionCount)
    //     {
    //         return; // Exit if no valid segment to change or if the waypoint is out of order
    //     }
    //
    //     // Get the collision point
    //     Vector3 collisionPoint = collision.contacts[0].point;
    //
    //     // Get the expected waypoint position
    //     Vector3 expectedWaypointPosition = lineController.points[lineController.currentPointIndex].position;
    //
    //     // Check if the collision point is close enough to the expected waypoint
    //     if (Vector3.Distance(collisionPoint, expectedWaypointPosition) < 0.1f)
    //     {
    //         // Move to the next waypoint and update the Line Renderer
    //         lineController.CheckWaypointReached();
    //     }
    // }
}