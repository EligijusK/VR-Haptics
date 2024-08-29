using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachObject : MonoBehaviour
{
    public Transform attachmentPoint; // The empty GameObject created as the attachment point
    

    void Start()
    {
        
    }

    private void OnInteract()
    {
        SnapToAttachmentPoint();
    }

    public void SnapToAttachmentPoint()
    {
        if (attachmentPoint != null)
        {
            // Snap the object's position and rotation to the attachment point
            transform.position = attachmentPoint.position;
            transform.rotation = attachmentPoint.rotation;
        }
    }
}
