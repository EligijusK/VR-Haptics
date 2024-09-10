using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AttachObject : MonoBehaviour
{
    [SerializeField] Vector3 snapRotation;
    [SerializeField] GameObject parentObject;
    public UnityEvent<GameObject> OnObjectAttached;
    public UnityEvent<GameObject> OnDropAttached;
    private GameObject attachedObject;
    private Transform originalParent;
    // [SerializeField] Rigidbody attachedRigidbody;

    void Update()
    {
        // Check the distance between the object's position and the target point
        // float distance = Vector3.Distance(transform.position, targetPoint.position);
        //
        // if (distance < snapDistance && !isSnapping)
        // {
        //     isSnapping = true;
        //     StartCoroutine(SnapObject());
        // }
    }
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Tampon" && attachedObject == null)
        {
            Destroy(other.attachedRigidbody);
            originalParent = other.transform.parent;
            other.transform.parent = transform;
            other.transform.position = transform.position;
            other.transform.rotation = Quaternion.Euler(snapRotation);
            attachedObject = other.gameObject;
            OnObjectAttached.Invoke(attachedObject);
            // parentObject.AddComponent<RaycastPainting>();

            // other.attachedRigidbody. = attachedRigidbody;
            // other.attachedRigidbody.isKinematic = false;
            // other.attachedRigidbody.useGravity = false;
            // other.attachedRigidbody.constraints = RigidbodyConstraints.FreezeAll;
        }
    }
    
    public void ReleaseObject()
    {
        if (attachedObject != null)
        {
            OnDropAttached.Invoke(attachedObject);
            attachedObject.transform.parent = originalParent;
            attachedObject = null;
            originalParent = null;
            
        }
    }
}
