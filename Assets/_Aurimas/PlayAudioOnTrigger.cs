using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAudioOnTrigger : MonoBehaviour
{
    private bool hasTriggered = false;
    
    public Color defaultColor = Color.white;
    
    public Color triggeredColor = Color.red;
    
    public GameObject[] objectsToChange;
    
    private List<Renderer> objectRenderers = new List<Renderer>();

    private void Awake()
    {
        foreach (GameObject obj in objectsToChange)
        {
            if (obj != null)
            {
                Renderer rend = obj.GetComponent<Renderer>();
                if (rend != null)
                {
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
            
            foreach (Renderer rend in objectRenderers)
            {
                if (rend != null)
                {
                    rend.material.color = triggeredColor;
                }
            }
            
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