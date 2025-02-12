using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAudioOnTrigger : MonoBehaviour
{
    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!hasTriggered)
        {
            hasTriggered = true;
        }
    }
}
