using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAudioOnTrigger : MonoBehaviour
{
    private bool hasTriggered = false;
    
    public Color defaultColor = Color.white;
    
    public Color triggeredColor = Color.red;
    
    public GameObject[] objectsToChange;

    // Store references to the renderers on the objects
    private List<Renderer> objectRenderers = new List<Renderer>();

    private void Awake()
    {
        // Loop through each object and cache its Renderer component.
        // Also, ensure each material instance is unique by instantiating a new material.
        foreach (GameObject obj in objectsToChange)
        {
            if (obj != null)
            {
                Renderer rend = obj.GetComponent<Renderer>();
                if (rend != null)
                {
                    // Create a unique material instance if you plan on changing its color independently.
                    rend.material = new Material(rend.material);
                    rend.material.color = defaultColor;
                    objectRenderers.Add(rend);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!hasTriggered)
        {
            hasTriggered = true;

            // Change the color of each object's material.
            foreach (Renderer rend in objectRenderers)
            {
                if (rend != null)
                {
                    rend.material.color = triggeredColor;
                }
            }
            
            // Start the audio sequence.
            StartCoroutine(PlayTwoClipsInSequence());
        }
    }

    private IEnumerator PlayTwoClipsInSequence()
    {
        // Play the first audio clip.
        AudioManager.Instance.EnterOperationRoom();
        
        // Wait until the first clip is done playing.
        while (AudioManager.Instance.IsAudioPlaying(0))
        {
            yield return null;
        }
        
        // Play the second audio clip.
        AudioManager.Instance.ChooseAntiseptic();
    }
}