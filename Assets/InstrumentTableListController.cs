using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstrumentTableListController : MonoBehaviour
{
    [SerializeField] private List<InstrumentSpot> spotsForInstruments;

    public void PlaceInstrument(Instrument instrumentToPlace)
    {
        foreach (var spot in spotsForInstruments)
        {
            if (spot.CanPlaceInSpot())
            {
                spot.PlaceInstrumentInSpot(instrumentToPlace);
                instrumentToPlace.spot = spot;
                break;
            }
        }
    }
}
