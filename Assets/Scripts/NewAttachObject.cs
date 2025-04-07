using System;
using UnityEngine;
using UnityEngine.Events;

public class NewAttachObject : MonoBehaviour
{
    [SerializeField] Vector3 snapRotation;
    public UnityEvent<GameObject> OnObjectAttached;

    private GameObject attachedObject;
    private Collider attachedCollider;

    private void OnTriggerEnter(Collider other)
    {
        if (attachedObject == null && other.attachedRigidbody != null && other.attachedRigidbody.CompareTag("Tampon"))
        {
            GameObject objectToAttach = other.attachedRigidbody.gameObject;
            attachedCollider = other;
            attachedObject = objectToAttach;

            // Attach the object automatically
            AttachNow();
        }
    }

    private void AttachNow()
    {
        if (attachedObject != null)
        {
            Destroy(attachedCollider.attachedRigidbody); // Optional: Remove Rigidbody if you want to freeze it
            attachedObject.transform.SetParent(transform);
            attachedObject.transform.position = transform.position;
            attachedObject.transform.rotation = Quaternion.Euler(snapRotation);
            OnObjectAttached.Invoke(attachedObject);
        }
    }
}
