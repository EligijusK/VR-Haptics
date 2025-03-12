using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DisplayTime : MonoBehaviour
{
    public TMP_Text timeText;
    void Start()
    {
        float finalTime = TimerManager.Instance.GetElapsedTime();
        
        timeText.text = $"Atlikote operacija per: {finalTime:F2} ";
    }
}
