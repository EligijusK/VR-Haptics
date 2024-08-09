using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstrumentSpot : MonoBehaviour
{
    private Instrument instrument;
    private bool taken;

    public bool CanPlaceInSpot()
    {
        return !taken;
    }

    public void PlaceInstrumentInSpot(Instrument instrumentToPlace)
    {
        instrument = instrumentToPlace;
        taken = true;
        StartCoroutine(MoveInstrumentToSpot(instrumentToPlace));
    }

    public void RemoveInstrumentFromSpot()
    {
        taken = false;
    }

    private IEnumerator MoveInstrumentToSpot(Instrument instrumentToPlace)
    {
        Vector3 startPosition = instrumentToPlace.transform.position;
        Vector3 endPosition = this.transform.position;
        float duration = 1.0f; // Adjust this for speed
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            float height = -4 * t * (t - 1); // This gives the parabolic effect

            Vector3 currentPosition = Vector3.Lerp(startPosition, endPosition, t);
            currentPosition.y += height;

            instrumentToPlace.transform.position = currentPosition;

            yield return null;
        }

        instrumentToPlace.transform.position = endPosition;
    }
    
}
