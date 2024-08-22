using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.Serialization;

public class Instrument : MonoBehaviour
{
    [SerializeField] public InstrumentTableListController table;
    [SerializeField] public Categories category;
    [SerializeField] public GameObject targetPositionObject;
    private Rigidbody _rigidbody;
    public InstrumentSpot spot;
    public Vector3 originalPosition;
    public Quaternion originalRotation;
    private bool onTable;
    private bool isMoving;
    private int interactions = 0;
    

    void Start()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void InteractWithItem()
    {
        if (isMoving) return;

        if (interactions > 0)
        {
            StartCoroutine(GetComponent<PouringAnimation>().MoveToPouringPosition(targetPositionObject, 1.0f, 0.5f, 1.0f));
            interactions++;
            return;
        }
        interactions++;
        
        if (!onTable)
        {
            onTable = table.TryPlaceInstrument(this);
        }
        else
        {
            spot.RemoveInstrumentFromSpot();
            onTable = false;
        }
    }

    public IEnumerator MoveInstrumentToSpot(Vector3 endPosition)
    {
        isMoving = true;
        transform.rotation = originalRotation;
        Vector3 startPosition = transform.position;
        float duration = 1.0f;
        float elapsedTime = 0f;
        
        _rigidbody.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            float height = -4 * t * (t - 1);

            Vector3 currentPosition = Vector3.Lerp(startPosition, endPosition, t);
            currentPosition.y += height;

            transform.position = currentPosition;

            yield return null;
        }

        transform.position = endPosition;
        
        yield return new WaitForSeconds(0.5f);
        transform.rotation = originalRotation;
        _rigidbody.constraints = RigidbodyConstraints.None;

        isMoving = false;
    }
}