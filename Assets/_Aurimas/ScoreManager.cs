using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ScoreManager : MonoBehaviour
{
    public static int score = 0;
    public static int expectedOrder = 1;
    
    public TMP_Text scoreText;
    
    public static void UpdateScore(int value)
        {
            score += value;
            Debug.Log("Current Score: " + score);
        }
    
        // Call this after a successful pickup to update the expected order.
        public static void AdvanceOrder()
        {
            expectedOrder++;
            Debug.Log("Next expected order: " + expectedOrder);
        }
    
        // Update UI every frame (if needed).
        void Update()
        {
            if(scoreText != null)
                scoreText.text = "Score: " + score;
        }
}
