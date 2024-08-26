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
                    FailedToPickInstrument("Incorrect item");
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
                FailedToPickInstrument("Picked too many of the same item");
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
        StartCoroutine(TextNotification._instance.ShowNotification("All items chosen correctly", 4.0f));
        /*text.gameObject.SetActive(true);
        text.text = "All items chosen correctly";
        trackMainCamera.StartTracking();
        yield return new WaitForSeconds(4.0f);
        text.gameObject.SetActive(false);*/
    }

    public void FailedToPickInstrument(string displayText)
    {
        StartCoroutine(TextNotification._instance.ShowNotification("All items chosen bad", 4.0f));

        /*text.gameObject.SetActive(true);
        text.text = displayText + --attemptsLeft + " attempts left";
        trackMainCamera.StartTracking();
        yield return new WaitForSeconds(4.0f);
        text.gameObject.SetActive(false);*/
    }
}
