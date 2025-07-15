using System;
using System.Collections;
using System.Collections.Generic;
using Es.InkPainter.Sample;
using UnityEngine;

public class TamponInAntiseptics : MonoBehaviour
{
    [SerializeField]
    float availableDistance = 0.1f;
    bool wasUsed = false;
    
    public void DipInAntiseptics()
    {
        MousePainter.painter.ResetPainting();
        wasUsed = false;
        Debug.Log("dipped");
    }

    public float GetAvailableDistance()
    {
        return availableDistance;
    }
    
    public bool CanTamponBeUsed()
    {
        return !wasUsed;
    }
    
    public void TamponWasUsed()
    {
        wasUsed = true;
        Debug.Log("Naudotas");
    }
}
