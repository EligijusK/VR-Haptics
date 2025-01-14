using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class HoverScale : MonoBehaviour
{
    public float scaleFactor = 1.2f;
    
    public float transitionSpeed = 10f;

    private Vector3 originalScale;
    private Vector3 targetScale;

    private void Awake()
    {
        originalScale = transform.localScale;
        targetScale = originalScale; 
    }

    private void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * transitionSpeed);
    }
    
    public void OnPointerEnter()
    {
        targetScale = originalScale * scaleFactor;
    }

    public void OnPointerExit()
    {
        targetScale = originalScale;
    }
    
}
