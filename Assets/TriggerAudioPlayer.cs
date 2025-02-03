using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerAudioPlayer : MonoBehaviour
{
    private AudioSource audioSource;
    private AudioSource audioSource2;
    private bool hasPlayed = false;  // Tracks if audio has already played

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource2 = GetComponent<AudioSource>();
        if (audioSource.clip == null)
        {
            Debug.LogWarning($"[{name}] No AudioClip assigned to AudioSource. Please assign a clip.", gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // If the audio hasn't played yet, play it and mark it as played
        if (!hasPlayed)
        {
            Debug.Log($"[{name}] Player entered door trigger, playing audio clip...");
            audioSource.Play();
            audioSource2.Play();
            hasPlayed = true;  // Ensures audio won't play again
        }
    }
}
