using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public int orderTag = 1;
    
    // This method should be called when the player interacts with the item.
    public void OnPickup()
    {
        // Check if the item is being picked up in the correct order.
        if(orderTag <= ScoreManager.expectedOrder)
        {
            // Correct order: gain a point and update expected order.
            ScoreManager.UpdateScore(1);
            ScoreManager.AdvanceOrder();
            Debug.Log("Correct item picked up!");
        }
        else
        {
            // Incorrect order: lose a point.
            ScoreManager.UpdateScore(-1);
            Debug.Log("Wrong order! Penalty applied.");
        }
        
        
        gameObject.SetActive(false);
    }
}
