using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeArt.Components;
using WeArt.Core;

public class WeArtTemperatureVolume : MonoBehaviour
{
    [SerializeField, Range(0f, 1f)]
    private float _volumeTemperature = 0.5f;

    private List<TouchableInVolume> touchablesInsideTheVolume = new List<TouchableInVolume>();

    private struct TouchableInVolume
    {
        public WeArtTouchableObject touchableObject;
        public float initialTemperature;
        public bool initialActiveTemperature;
    }

    public float VolumeTemperature
    {
        get { return _volumeTemperature; }
        set { _volumeTemperature = value; }
    }

    void Start()
    {
        
    }

    
    void Update()
    {
        
    }

    private void OnDisable()
    {
        ReverseTouchableObjectsToOriginal();
    }

    private void OnDestroy()
    {
        ReverseTouchableObjectsToOriginal();
    }

    private void ReverseTouchableObjectsToOriginal()
    {
        foreach (var touch in touchablesInsideTheVolume)
        {
            if (touch.touchableObject == null)
                continue;

            Temperature temp = touch.touchableObject.Temperature;
            temp.Value = touch.initialTemperature;
            temp.Active = touch.initialActiveTemperature;
            touch.touchableObject.Temperature = temp;
        }
        touchablesInsideTheVolume.Clear();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (TryGetTouchableObjectFromCollider(other, out WeArtTouchableObject touchable))
        {
            bool found = false;
            foreach (var touch in touchablesInsideTheVolume)
            {
                if(touch.touchableObject == touchable)
                    found = true;
            }

            if (!found)
            {
                touchablesInsideTheVolume.Add(new TouchableInVolume { touchableObject = touchable, initialTemperature = touchable.Temperature.Value, 
                initialActiveTemperature = touchable.Temperature.Active});
                Temperature temp = touchable.Temperature;
                temp.Value = _volumeTemperature;
                temp.Active = true;
                touchable.Temperature = temp;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {

    }

    private void OnTriggerExit(Collider other)
    {
        if(TryGetTouchableObjectFromCollider(other, out WeArtTouchableObject touchable))
        {
            foreach(var touch in touchablesInsideTheVolume)
            {
                if (touch.touchableObject == touchable)
                {
                    Temperature temp = touchable.Temperature;
                    temp.Value = touch.initialTemperature;
                    temp.Active = touch.initialActiveTemperature;
                    touchable.Temperature = temp;
                    touchablesInsideTheVolume.Remove(touch);
                    break;
                }
            }
        }
    }

    private static bool TryGetTouchableObjectFromCollider(Collider collider, out WeArtTouchableObject touchableObject)
    {
        touchableObject = collider.gameObject.GetComponent<WeArtTouchableObject>();
        return touchableObject != null;
    }


}
