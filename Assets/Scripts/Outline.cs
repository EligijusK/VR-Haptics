using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Outline : MonoBehaviour
{
    private static readonly int OutlineEnabled = Shader.PropertyToID("_OutlineEnabled");
    
    [SerializeField] float blinkTime = 10.0f;
    Material _material;
    Renderer _renderer;
    bool _isBlinking = false;
    bool _isOutlineEnabled = false;
    bool _originalOutlineState = false;
    void Start()
    {
        _renderer = GetComponent<Renderer>();
        _material = _renderer.materials[0];
        DisableOutline();
    }
    
    public void EnableOutline()
    {
        StopBlinking();
        EnableOutlineWithoutState();
        _originalOutlineState = true;
    }
    
    public void DisableOutline()
    {
        StopBlinking();
        DisableOutlineWithoutState();
        _originalOutlineState = true;
    }
    
    private void DisableOutlineWithoutState()
    {
        _material.SetFloat(OutlineEnabled, 0);
        _isOutlineEnabled = false;
    }
    
    private void EnableOutlineWithoutState()
    {
        _material.SetFloat(OutlineEnabled, 1);
        _isOutlineEnabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void StartBlinking()
    {
        _isBlinking = true;
        StartCoroutine(Blinking());
    }
    
    public void StartBlinkingTimer()
    {
        _isBlinking = true;
        StartCoroutine(BlinkingForTime());
    }
    
    public void StopBlinking()
    {
        _isBlinking = false;
    }
    
    private IEnumerator Blinking()
    {
        while (_isBlinking)
        {
            EnableOutlineWithoutState();
            yield return new WaitForSeconds(0.5f);
            DisableOutlineWithoutState();
            yield return new WaitForSeconds(0.5f);
        }

        if (_originalOutlineState)
        {
            EnableOutlineWithoutState();
        }
        else
        {
            DisableOutlineWithoutState();
        }
    }
    
    private IEnumerator BlinkingForTime()
    {
        float timer = 0;
        while (_isBlinking && blinkTime > timer)
        {
            EnableOutlineWithoutState();
            timer += 0.5f;
            yield return new WaitForSeconds(0.5f);
            DisableOutlineWithoutState();
            timer += 0.5f;
            yield return new WaitForSeconds(0.5f);
        }
        _isBlinking = false;
        if (_originalOutlineState)
        {
            EnableOutlineWithoutState();
        }
        else
        {
            DisableOutlineWithoutState();
        }
    }
}
