using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using CommonUsages = UnityEngine.XR.CommonUsages;
using InputDevice = UnityEngine.XR.InputDevice;

public class TriggerEventSystem : MonoBehaviour
{
    public delegate void TriggerEventHandler(string handName);
    public static event TriggerEventHandler OnTriggerPressed;
    public static event TriggerEventHandler OnTriggerReleased;
    [SerializeField]
    InputActionReference m_triggerLeft;
    [SerializeField]
    InputActionReference m_triggerRight;

    private InputDevice leftHandDevice;
    private InputDevice rightHandDevice;
    
   

    void Start()
    {
        InitializeDevices();
        m_triggerLeft.action.started += CheckTriggerPress;
        m_triggerRight.action.started += CheckTriggerPress;
    }

    void InitializeDevices()
    {
        var inputDevices = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Controller, inputDevices);

        foreach (var device in inputDevices)
        {
            if (device.characteristics.HasFlag(InputDeviceCharacteristics.Left))
            {
                leftHandDevice = device;
            }
            else if (device.characteristics.HasFlag(InputDeviceCharacteristics.Right))
            {
                rightHandDevice = device;
            }
        }
    }

    void Update()
    {
        
    }

    void CheckTriggerPress(InputAction.CallbackContext context)
    {
        Debug.Log("Aurimas Ciulpia");
    }
}
