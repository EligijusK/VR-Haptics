using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class InstrumentTableListController : MonoBehaviour
{
    [SerializeField] private List<InstrumentSpot> spotsForInstruments;
    [SerializeField] private List<InstrumentCategory> categories;

    public bool TryPlaceInstrument(Instrument instrumentToPlace)
    {
        foreach (var category in categories)
        {
            if (category.category.Equals(instrumentToPlace.category))
            {
                if (category.category == Categories.Default)
                {
                    //picked wrong object
                    return false;
                }
                if (category.currentCount < category.requiredCount)
                {
                    PlaceInstrument(instrumentToPlace);
                    category.currentCount++;
                    return true;
                }
                //picked too many objects of the same category
                return false;
            }
        }
        return false;
    }
    
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
