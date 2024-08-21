using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class LineCollisionHandler : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
       // Debug.Log("Collision detected with: " + collision.gameObject.name);
        // Handle collision
    }

    void OnCollisionStay(Collision collision)
    {
       // Debug.Log("Continuing collision with: " + collision.gameObject.name);
        // Handle ongoing collision
    }

    void OnCollisionExit(Collision collision)
    {
        //Debug.Log("Collision ended with: " + collision.gameObject.name);
        // Handle collision end
    }
}
