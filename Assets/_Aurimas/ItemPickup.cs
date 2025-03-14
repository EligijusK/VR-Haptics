using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public void OnPickup(bool isCorrectTime)
    {
        if (isCorrectTime)
        {
            ScoreManager.UpdateScore(1);
        }
        else
        {
            ScoreManager.UpdateScore(-1);
        }
        
    }
}
