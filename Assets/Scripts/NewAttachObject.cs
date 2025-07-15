using System;
using UnityEngine;
using UnityEngine.Events;

// 1) Declare this at namespace scope (outside of your MonoBehaviour)
[Serializable]
public class GameObjectEvent : UnityEvent<GameObject> { }

public class NewAttachObject : MonoBehaviour
{
    [SerializeField] Vector3 snapRotation;
    // 2) Use your serializable subclass here
    public GameObjectEvent OnObjectAttached;

    private GameObject attachedObject;
    private Collider attachedCollider;

    private void OnTriggerEnter(Collider other)
    {
        if (attachedObject == null 
            && other.attachedRigidbody != null 
            && other.attachedRigidbody.CompareTag("Tampon"))
        {
            attachedCollider = other;
            attachedObject = other.attachedRigidbody.gameObject;
            AttachNow();
        }
    }

    private void AttachNow()
    {
        if (attachedObject == null) return;

        Destroy(attachedCollider.attachedRigidbody);
        attachedObject.transform.SetParent(transform);
        attachedObject.transform.position = transform.position;
        attachedObject.transform.rotation = Quaternion.Euler(snapRotation);
        OnObjectAttached.Invoke(attachedObject);
    }
}