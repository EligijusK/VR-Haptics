using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
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
        StartCoroutine(instrument.MoveInstrumentToSpot(transform.position));
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
}
