using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestAudioOnTrigger : MonoBehaviour
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
