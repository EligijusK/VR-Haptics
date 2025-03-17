using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ScoreManager : MonoBehaviour
{
    public static int score = 0;
    
    public Text scoreText;
    
    public static void UpdateScore(int value)
    {
        score += value;
        Debug.Log("Current Score: " + score);
    }
    
    public void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }
    void Update()
    {
        UpdateScoreUI();
    }
}
