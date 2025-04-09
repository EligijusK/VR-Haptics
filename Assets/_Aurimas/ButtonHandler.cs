using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonHandler : MonoBehaviour
{
    public ScoreManager scoreManager;
    public void OnButtonPress(bool isCorrect)
    {
        if (isCorrect)
        {
            ScoreManager.UpdateScore(1);
        }
        else
        {
            //ScoreManager.UpdateScore(-1); 
            scoreManager.HandleWrongAnswer();
        }
    }
}
