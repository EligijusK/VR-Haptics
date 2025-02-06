using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR.Interaction.Toolkit;
// using UnityEngine.XR.Interaction.Toolkit.Inputs.;

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
    }


    public virtual void OnPlace()
    {
        SimpleInteractable.enabled = false;
        GrabInteractable.enabled = true;
        InstrumentProgressTracker._instance.InstrumentPlaced();
    }
}