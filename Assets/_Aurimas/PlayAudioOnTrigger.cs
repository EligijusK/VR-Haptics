using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAudioOnTrigger : MonoBehaviour
{
    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        // Prevent multiple triggers if you only want it once
        if (!hasTriggered)
        {
            hasTriggered = true;
            StartCoroutine(PlayTwoClipsInSequence());
        }
    }

    private System.Collections.IEnumerator PlayTwoClipsInSequence()
    {
        // 1) Play the first audio
        AudioManager.Instance.EnterOperationRoom();

        // 2) Wait until that audio stops playing
        while (AudioManager.Instance.IsAudioPlaying(0))
        {
            yield return null; 
        }

        // 3) Now play the second audio
        AudioManager.Instance.ChooseAntiseptic();
        
        // Optionally, if you want to wait for the second clip to finish too:
        // while (AudioManager.Instance.IsAudioPlaying(1))
        // {
        //     yield return null; 
        // }
    }
}
