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
    private Collider attachedCollider;
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
        if (other.attachedRigidbody != null && other.attachedRigidbody.CompareTag("Tampon") && attachedObject == null)
        {
            GameObject objectToAttach = other.attachedRigidbody.gameObject;
            attachedCollider = other;
            attachedObject = objectToAttach;
            // parentObject.AddComponent<RaycastPainting>();

            // other.attachedRigidbody. = attachedRigidbody;
            // other.attachedRigidbody.isKinematic = false;
            // other.attachedRigidbody.useGravity = false;
            // other.attachedRigidbody.constraints = RigidbodyConstraints.FreezeAll;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.attachedRigidbody != null && other.attachedRigidbody.CompareTag("Tampon") && attachedObject != null)
        {
            attachedCollider = null;
            attachedObject = null;
            originalParent = null;
        }
    }

    public void TriggerAttachObject()
    {
        Debug.Log("TriggerAttachObject");
        if (attachedObject != null)
        {
            Destroy(attachedCollider.attachedRigidbody);
            originalParent = attachedObject.transform.parent;
            attachedObject.transform.parent = transform;
            attachedObject.transform.position = transform.position;
            attachedObject.transform.rotation = Quaternion.Euler(snapRotation);
            OnObjectAttached.Invoke(attachedObject);
        }
    }

    public void ReleaseObject()
    {
        if (attachedObject != null)
        {
            GameObject releasedObject = attachedObject;
            attachedObject.transform.parent = originalParent;
            attachedObject.AddComponent<Rigidbody>();
            attachedCollider = null;
            attachedObject = null;
            originalParent = null;
            Debug.Log("ReleaseObject");
            OnDropAttached.Invoke(releasedObject);
            
        }
    }
}
