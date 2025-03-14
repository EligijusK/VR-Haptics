using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ScoreManager : MonoBehaviour
{
    public static int score = 0;

    // Optionally, if you have a UI text to display the score:
    public Text scoreText;

    // Call this method to update the score.
    public static void UpdateScore(int value)
    {
        score += value;
        Debug.Log("Current Score: " + score);
    }

    // Example instance method for updating the UI.
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
