using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR.Interaction.Toolkit;

public class Instrument : MonoBehaviour
{
    [SerializeField] public InstrumentTableListController table;
    [SerializeField] public Categories category;
    [SerializeField] public GameObject targetPositionObject;
    [SerializeField] public XRSimpleInteractable SimpleInteractable;
    [SerializeField] public XRGrabInteractable GrabInteractable;
    [SerializeField] public Transform predeterminedSpot;
    private Rigidbody _rigidbody;
    public Vector3 originalPosition;
    public Quaternion originalRotation;
    protected bool onTable;
    protected bool isMoving;

    public virtual void Start()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        _rigidbody = GetComponent<Rigidbody>();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("FloorAndWalls"))
        {
            ResetInstrumentPosition();
        }
    }
    
    private void ResetInstrumentPosition()
    {
        if (_rigidbody != null)
        {
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
        }

        transform.position = originalPosition;
        transform.rotation = originalRotation;
        
        // Make the item kinematic for 0.2 seconds to fully reset it
        StartCoroutine(TemporaryKinematicReset());
        
        if (GrabInteractable != null)
        {
            GrabInteractable.enabled = false;
        }
        SimpleInteractable.enabled = true;
        onTable = false;
        table.RemoveInstrument(this);
        AudioManager.Instance.FallenInstrument();
    }
    
    private IEnumerator TemporaryKinematicReset()
    {
        if (_rigidbody != null)
        {
            _rigidbody.isKinematic = true;
        }
        
        yield return new WaitForSeconds(1.5f);
        
        if (_rigidbody != null)
        {
            _rigidbody.isKinematic = false;
        }
    }

    public virtual void InteractWithItem()
    {
        if (isMoving) return;
        
        if (!onTable)
        {
            onTable = table.TryPlaceInstrument(this);
        }
        else
        {
            //removed ability to remove from table
            //spot.RemoveInstrumentFromSpot();
            //onTable = false;
        }
    }

    public IEnumerator MoveInstrumentToSpot(Vector3 endPosition)
    {
        StartCoroutine(TemporaryKinematicReset());
        isMoving = true;
        transform.rotation = originalRotation; 
        Vector3 startPosition = transform.position;  
        float duration = 1.0f;
        float elapsedTime = 0f;

        _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            float height = -4 * t * (t - 1); 
            Vector3 currentPosition = Vector3.Lerp(startPosition, endPosition, t);
            currentPosition.y += height; 
            _rigidbody.MovePosition(currentPosition);
            yield return null;  
        }
        _rigidbody.MovePosition(endPosition);
        yield return new WaitForSeconds(0.5f);
        transform.rotation = originalRotation;  
        _rigidbody.constraints = RigidbodyConstraints.None;  
        isMoving = false;  
        OnPlace();
    }

    public virtual void OnPlace()
    {
        // Update the original position to the current position when placed on table
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        
        SimpleInteractable.enabled = false;
        if (GrabInteractable != null)
        {
            GrabInteractable.enabled = true;
        }
        // Unlock Rigidbody rotation and position along all axes
        if (_rigidbody != null)
        {
            _rigidbody.constraints = RigidbodyConstraints.None;
        }
        InstrumentProgressTracker._instance.InstrumentPlaced();
    }
}