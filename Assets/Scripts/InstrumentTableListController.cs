using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class InstrumentTableListController : MonoBehaviour
{
    [SerializeField] private List<InstrumentSpot> spotsForInstruments;
    [SerializeField] public List<InstrumentCategory> categories;
    private int totalRequired = 0;
    private int currentTotalCount = 0;

    private void Start()
    {
        foreach (var category in categories)
        {
            totalRequired += category.requiredCount;
        }
    }

    public bool TryPlaceInstrument(Instrument instrumentToPlace)
    {
        foreach (var category in categories)
        {
            if (category.category.Equals(instrumentToPlace.category))
            {
                if (category.category == Categories.Default)
                {
                    //picked wrong object
                    FailedToPickInstrument();
                    return false;
                }
                if (category.currentCount < category.requiredCount)
                {
                    PlaceInstrument(instrumentToPlace);
                    category.currentCount++;
                    currentTotalCount++;
                    if (currentTotalCount == totalRequired)
                    {
                        SuccessfullyPlacedAllInstruments();
                    }
                    return true;
                }
                //picked too many objects of the same category
                FailedToPickInstrument();
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

    public void SuccessfullyPlacedAllInstruments()
    {
        throw new NotImplementedException();
    }

    public void FailedToPickInstrument()
    {
        throw new NotImplementedException();

    }
}
