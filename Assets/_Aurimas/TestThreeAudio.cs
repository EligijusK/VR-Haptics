using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestThreeAudio : MonoBehaviour
{
    public AudioSource audioSource;
    
    private bool hasPlayed = false;

    private void Start()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
        
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (!hasPlayed && audioSource != null)
        {
            audioSource.Play();
            hasPlayed = true;
        }
    }
}
