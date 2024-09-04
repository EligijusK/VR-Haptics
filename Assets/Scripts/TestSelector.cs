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
            label.text = "eik nx";
        }
        else
        {
            Debug.Log(incorrectMessage);
            label.text = "pisk nx";
        }
    }

    public void PopUp()
    {
        StartCoroutine(TextNotification._instance.ShowNotification("eik nx", 3.0f));
    }
}
