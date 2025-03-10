using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InteractAndLoadScene : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (TimerManager.Instance != null)
        {
            TimerManager.Instance.StopTimer();
            Debug.Log("Veikia galimai");
        }
        SceneManager.LoadScene("ExitScene");
    }
}