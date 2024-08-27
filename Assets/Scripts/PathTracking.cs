using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathTracking : MonoBehaviour
{
    public Transform[] waypoints; // Array of waypoints
    public float raycastDistance = 2.0f; // The distance of the raycast for detecting waypoints
    public Color rayColor = Color.red; // The color of the ray (visible in the Scene view)

    private int currentWaypointIndex = 0; // Index to track the current waypoint
    private HashSet<int> visitedWaypoints = new HashSet<int>(); // Track visited waypoints

    private LineController lineController; // Reference to the LineController script

    void Start()
    {
        // Find and reference the LineController script
        lineController = GetComponent<LineController>();

        if (lineController == null)
        {
            Debug.LogError("LineController script not found on the same GameObject as PathTracking!");
        }
    }

    void Update()
    {
        // Continuously check if the object is facing the current waypoint
        if (currentWaypointIndex < waypoints.Length)
        {
            CheckWaypointWithRaycast(); // Check if the object is close enough to the current waypoint using raycast
        }
    }

    void CheckWaypointWithRaycast()
    {
        // Cast a ray from the object's position forward
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        // Visualize the ray using Debug.DrawRay
        Debug.DrawRay(ray.origin, ray.direction * raycastDistance, rayColor);

        // If the ray hits something within the specified distance
        if (Physics.Raycast(ray, out hit, raycastDistance))
        {
            // Check if the ray hit the expected waypoint
            if (hit.transform == waypoints[currentWaypointIndex])
            {
                // Ensure the waypoint hasn't been visited yet
                if (!visitedWaypoints.Contains(currentWaypointIndex))
                {
                    // Mark this waypoint as visited
                    visitedWaypoints.Add(currentWaypointIndex);

                    // Notify LineController to extend the line
                    lineController.ExtendLineRenderer(hit.transform.position);

                    Debug.Log("Waypoint reached: " + currentWaypointIndex + " Position: " + waypoints[currentWaypointIndex].position);

                    // Move to the next waypoint
                    currentWaypointIndex++;
                }
            }
        }
    }
}