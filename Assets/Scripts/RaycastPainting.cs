using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class RaycastPainting : MonoBehaviour
{
    [SerializeField] private Transform raycastOrigin;
    [SerializeField] private float hitDistance = 0.05f;
    [SerializeField] private Vector2 brushSizeRange = new Vector2(0.01f, 0.1f);
    [SerializeField] private Vector2 minMaxDistance = new Vector2(0.01f, 0.05f);
    int callsPerSecond = 8000;
    RaycastHit hit;
    bool wasHit = false;
    Vector3 hitPoint;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        // Debug.DrawRay(raycastOrigin.position, raycastOrigin.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
        if (Physics.Raycast(raycastOrigin.position, raycastOrigin.TransformDirection(Vector3.forward), out hit, Mathf.Infinity) &&
            hit.distance < hitDistance && hit.transform.CompareTag("PaintObject") && !wasHit)
        {
            wasHit = true;
            // Debug.Log("Hit PaintObject");
            hitPoint = hit.point;
        }
        else
        {
            wasHit = false;
        }
    }
    

    // private void OnCollisionEnter(Collision other)
    // {
    //     // hitPoint = other.GetContact(0).point;
    //     wasHit = true;
    // }
    //
    // private void OnCollisionExit(Collision other)
    // {
    //     wasHit = false;
    // }

    
    public RaycastHit GetHit()
    {
        return hit;
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