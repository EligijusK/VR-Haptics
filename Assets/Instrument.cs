using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instrument : MonoBehaviour
{
    [SerializeField] private InstrumentTableListController table;
    public InstrumentSpot spot;
    public Vector3 originalPosition;
    private bool onTable;
    void Start()
    {
        originalPosition = transform.position;
    }
    
    public void PlaceOnTable()
    {
        if (!onTable)
        {
            table.PlaceInstrument(this);
            onTable = true;
        }
        else
        {
            spot.RemoveInstrumentFromSpot();
            onTable = false;
            StartCoroutine(MoveInstrumentToOriginalPosition());
        }
    }
    
    private IEnumerator MoveInstrumentToOriginalPosition()
    {
        Vector3 startPosition = transform.position;
        Vector3 endPosition = originalPosition;
        float duration = 1.0f; // Adjust this for speed
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            float height = -4 * t * (t - 1); // This gives the parabolic effect

            Vector3 currentPosition = Vector3.Lerp(startPosition, endPosition, t);
            currentPosition.y += height;

            transform.position = currentPosition;

            yield return null;
        }

        transform.position = endPosition;
    }
}
