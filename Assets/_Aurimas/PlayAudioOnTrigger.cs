using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAudioOnTrigger : MonoBehaviour
{
    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!hasTriggered)
        {
            hasTriggered = true;
            StartCoroutine(PlayTwoClipsInSequence());
        }
    }

    private IEnumerator PlayTwoClipsInSequence()
    {
        AudioManager.Instance.EnterOperationRoom();
        
        while (AudioManager.Instance.IsAudioPlaying(0))
        {
            yield return null; 
        }
        
        AudioManager.Instance.ChooseAntiseptic();
        
    }
}
