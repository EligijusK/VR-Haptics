using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerEventListner : MonoBehaviour
{
    void OnEnable()
    {
        TriggerEventSystem.OnTriggerPressed += HandleTriggerPressed;
        TriggerEventSystem.OnTriggerReleased += HandleTriggerReleased;
    }

    void OnDisable()
    {
        TriggerEventSystem.OnTriggerPressed -= HandleTriggerPressed;
        TriggerEventSystem.OnTriggerReleased -= HandleTriggerReleased;
    }

    void HandleTriggerPressed(string handName)
    {
        Debug.Log(handName + " Trigger Pressed");
        // Perform actions when the trigger is pressed
    }

    void HandleTriggerReleased(string handName)
    {
        Debug.Log(handName + " Trigger Released");
        // Perform actions when the trigger is released
    }
}
