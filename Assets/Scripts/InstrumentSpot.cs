using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

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
        StartCoroutine(MoveAndMakeGrabbable(instrument));
    }
    
    private IEnumerator MoveAndMakeGrabbable(Instrument instrumentToPlace)
    {
        yield return StartCoroutine(instrumentToPlace.MoveInstrumentToSpot(transform.position));
        instrumentToPlace.MakeItemGrabbable();
    }

    public void RemoveInstrumentFromSpot()
    {
        taken = false;
        StartCoroutine(instrument.MoveInstrumentToSpot(instrument.originalPosition));
        Categories cat = instrument.category;
        InstrumentTableListController table = instrument.table;
        var category = table.categories.Find(c => c.category == cat);
        if (category != null)
        {
            category.currentCount--;
        }    
    }

    public Instrument GetInstrument()
    {
        return instrument;
    }
}
