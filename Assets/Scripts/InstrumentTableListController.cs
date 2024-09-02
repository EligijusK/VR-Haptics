using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

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
            if (!category.optional)
            {
                totalRequired += category.requiredCount;
            }
        }
    }
    

    public bool TryPlaceInstrument(Instrument instrumentToPlace)
    {
        foreach (var category in categories)
        {
            if (category.category.Equals(instrumentToPlace.category))
            {
                if (category.currentCount < category.requiredCount)
                {
                    PlaceInstrument(instrumentToPlace);
                    if (!category.optional)
                    {
                        category.currentCount++;
                        currentTotalCount++;
                        if (currentTotalCount == totalRequired)
                        {
                            SuccessfullyPlacedAllInstruments();
                        }
                    }
                    return true;
                }
                //picked too many objects of the same category
                FailedToPickInstrument();
                return false;
            }
        }
        FailedToPickInstrument();
        return false;
    }
    
    private void PlaceInstrument(Instrument instrumentToPlace)
    {
        foreach (var spot in spotsForInstruments)
        {
            if (spot.CanPlaceInSpot())
            {
                spot.PlaceInstrumentInSpot(instrumentToPlace);
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
        string displayText;
        float displayTime;
        if (attemptsLeft < 1)
        {
            displayText = "You have failed";
            displayTime = 1000.0f;
        }
        else
        {
            displayText = "Picked wrong instrument " + --attemptsLeft + " attempts left.";
            displayTime = 4.0f;
        }
        StartCoroutine(TextNotification._instance.ShowNotification(displayText, displayTime));
    }
}
