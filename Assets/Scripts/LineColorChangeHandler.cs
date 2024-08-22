using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineColorChangeHandler : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public Material collisionMaterial;  // Assign the material you want to change to upon collision
    public Material defaultMaterial;    // Assign the default material

    void OnCollisionEnter(Collision collision)
    {
        // Get the collision point
        Vector3 collisionPoint = collision.contacts[0].point;

        // Find the closest point on the line
        float closestDistance = float.MaxValue;
        int closestSegmentIndex = 0;

        for (int i = 0; i < lineRenderer.positionCount - 1; i++)
        {
            Vector3 segmentStart = lineRenderer.GetPosition(i);
            Vector3 segmentEnd = lineRenderer.GetPosition(i + 1);

            // Calculate the closest point on this segment
            Vector3 closestPoint = ClosestPointOnLineSegment(segmentStart, segmentEnd, collisionPoint);
            float distance = Vector3.Distance(collisionPoint, closestPoint);


            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestSegmentIndex = i;
            }
        }

        
        // Change the material of the line at the closest segment
        ChangeLineMaterialAtSegment(closestSegmentIndex);
    }

    Vector3 ClosestPointOnLineSegment(Vector3 a, Vector3 b, Vector3 point)
    {
        Vector3 ab = b - a;
        float t = Vector3.Dot(point - a, ab) / Vector3.Dot(ab, ab);
        t = Mathf.Clamp01(t);
        return a + t * ab;
    }

    void ChangeLineMaterialAtSegment(int segmentIndex)
    {
        // Assuming the line is segmented and you want to change the material of the closest segment
        // Unity's LineRenderer doesn't natively support changing material per segment.
        // To achieve this, you would need to split the LineRenderer into separate segments or use a shader/material that can handle this.

        // For simplicity, let's change the entire LineRenderer's material for now.
        // You can expand this to manage different materials for each segment if needed.
        
        lineRenderer.material = collisionMaterial;

        // If you want to revert back to the default material after a certain time, you can use a coroutine
        StartCoroutine(RevertMaterialAfterTime(2.0f)); // Reverts after 2 seconds
    }

    IEnumerator RevertMaterialAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        lineRenderer.material = defaultMaterial;
    }
}
