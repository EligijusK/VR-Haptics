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
    
    public void InteractWithItem()
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
        }
    }
    
    public IEnumerator MoveInstrumentToSpot(Vector3 endPosition)
    {
        Vector3 startPosition = transform.position;
        float duration = 1.0f;
        float elapsedTime = 0f;

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
    }
}
