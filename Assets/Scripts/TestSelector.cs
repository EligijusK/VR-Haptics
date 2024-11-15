using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using TMPro;
using UnityEngine;

public class TestSelector : MonoBehaviour
{
    // Assign these in the Unity Editor
    [SerializeField] public GameObject correctObject;
    [SerializeField] public TMP_Text label;
    public string correctMessage = "Good job!";
    public string incorrectMessage = "Bad job!";

    
    public void OnObjectSelected(GameObject selectedObject)
    {
        if (selectedObject == correctObject)
        {
            Debug.Log(correctMessage);
            label.text = correctMessage;
            label.text = correctMessage;
            Canvas.ForceUpdateCanvases();

        }
        else
        {
            Debug.Log(incorrectMessage);
            label.text = incorrectMessage;
            label.text = incorrectMessage;
            Canvas.ForceUpdateCanvases();
        }
    }

    public void PopUp()
    {
        StartCoroutine(TextNotification._instance.ShowNotification("Saunuolis", 3.0f));
    }
}
