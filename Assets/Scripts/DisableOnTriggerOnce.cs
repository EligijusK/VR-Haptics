using UnityEngine;

public class DisableOnTriggerOnce : MonoBehaviour
{
    [SerializeField] private GameObject objectToDisable; // Assign in Inspector
    private bool hasTriggered = false; // Ensures this only happens once

    private void OnTriggerEnter(Collider other)
    {
        if (!hasTriggered && objectToDisable != null)
        {
            objectToDisable.SetActive(false);
            hasTriggered = true; // Prevents future triggers
        }
        else if (objectToDisable == null)
        {
            Debug.LogWarning("No object assigned to disable!", this);
        }
    }
}