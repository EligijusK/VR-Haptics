using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TamponInAntyseptics : MonoBehaviour
{
    [SerializeField]
    float availableDistance = 0.1f;
    bool wasUsed = false;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Antiseptics") && wasUsed)
        {
            wasUsed = false;
            // Debug.Log("Tampon was in antyseptics");
        }
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
    }
}
