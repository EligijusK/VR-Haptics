using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class IndexControllerDetection : MonoBehaviour
{
    
    [SerializeField] GameObject leftHand;
    [SerializeField] GameObject rightHand;
    
    [SerializeField] GameObject leftController;
    [SerializeField] GameObject rightController;
    
    void OnEnable()
    {
        InputDevices.deviceConnected += DeviceConnected;
        List<InputDevice> devices = new List<InputDevice>();
        InputDevices.GetDevices(devices);
        foreach(var device in devices)
            DeviceConnected(device);
    }
    void OnDisable()
    {
        InputDevices.deviceConnected -= DeviceConnected;
    }
    void DeviceConnected(InputDevice device)
    {
        // The Left Hand
        if ((device.characteristics & InputDeviceCharacteristics.Left) != 0)
        {
            
            if (device.name == "Index Controller OpenXR")
            {
                leftHand.SetActive(true);
                leftController.SetActive(false);
            }

            //Use device.name here to identify the current Left Handed Device
        }
        // The Right hand
        else if ((device.characteristics & InputDeviceCharacteristics.Right) != 0)
        {
            if (device.name == "Index Controller OpenXR")
            {
                rightHand.SetActive(true);
                rightController.SetActive(false);
            }
            
            //Use device.Name here to identify the current Right Handed Device
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
