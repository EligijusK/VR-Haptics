using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachObject : MonoBehaviour
{
    [SerializeField] Vector3 snapRotation;
    [SerializeField] GameObject parentObject;
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
        if (other.tag == "Tampon")
        {
            Destroy(other.attachedRigidbody);
            other.transform.parent = transform;
            other.transform.position = transform.position;
            other.transform.rotation = Quaternion.Euler(snapRotation);
            // parentObject.AddComponent<RaycastPainting>();

            // other.attachedRigidbody. = attachedRigidbody;
            // other.attachedRigidbody.isKinematic = false;
            // other.attachedRigidbody.useGravity = false;
            // other.attachedRigidbody.constraints = RigidbodyConstraints.FreezeAll;
        }
    }
}
