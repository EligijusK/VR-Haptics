using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachObject : MonoBehaviour
{
    public Transform targetPoint;     // The target point where you want the object to snap
    public float snapDistance = 0.1f; // Distance within which snapping should occur
    public float snapSpeed = 10f;     // How fast the object should snap into place

    private bool isSnapping = false;

    void Update()
    {
        // Check the distance between the object's position and the target point
        float distance = Vector3.Distance(transform.position, targetPoint.position);

        if (distance < snapDistance && !isSnapping)
        {
            isSnapping = true;
            StartCoroutine(SnapObject());
        }
    }

    private IEnumerator SnapObject()
    {
        while (Vector3.Distance(transform.position, targetPoint.position) > 0.01f)
        {
            // Move the object towards the target point
            transform.position = Vector3.Lerp(transform.position, targetPoint.position, snapSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetPoint.rotation, snapSpeed * Time.deltaTime);
            yield return null;
        }

        // Ensure the object is exactly at the target point
        transform.position = targetPoint.position;
        transform.rotation = targetPoint.rotation;

        isSnapping = false;
    }
}
