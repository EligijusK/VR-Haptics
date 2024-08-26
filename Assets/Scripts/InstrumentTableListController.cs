using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using TMPro;
using UnityEngine;

public class InstrumentTableListController : MonoBehaviour
{
    [SerializeField] private List<InstrumentSpot> spotsForInstruments;
    [SerializeField] public List<InstrumentCategory> categories;
    [SerializeField] public int attemptsLeft = 3;
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

    private void SuccessfullyPlacedAllInstruments()
    {
        StartCoroutine(TextNotification._instance.ShowNotification("All items chosen correctly", 4.0f));
    }

    private void FailedToPickInstrument()
    {
        StartCoroutine(TextNotification._instance.ShowNotification("All items chosen bad", 4.0f));
    }
}
