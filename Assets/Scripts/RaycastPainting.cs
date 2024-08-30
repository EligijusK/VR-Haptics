using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class RaycastPainting : MonoBehaviour
{
    RaycastHit hit;
    bool wasHit = false;
    Vector3 hitPoint;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
        
        // Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity);
        // Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
        // if (hit.transform.tag == "PaintObject")
        // {
        //     Debug.Log("Hit PaintObject");
        //     wasHit = true;
        //     hitPoint = hit.point;
        // }
        // else
        // {
        //     wasHit = false;
        // }
    }

    private void OnCollisionStay(Collision other)
    {
        hitPoint = other.GetContact(0).point;
        wasHit = true;
    }

    private void OnCollisionExit(Collision other)
    {
        wasHit = false;
    }

    public bool GetWasHit()
    {
        return wasHit;
    }
    
    public Vector3 GetHitPoint()
    {
        return hitPoint;
    }
}
