using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WeArt.Components;
using WeArt.Core;
using WeArt.Messages;

public class WeArtDeviceStatusDisplay : MonoBehaviour
{
    [SerializeField]
    internal WeArtStatusTracker tracker;

    [SerializeField]
    internal HandSide _handSide = HandSide.Left;

    /// <summary>
    /// The hand side of the thimble
    /// </summary>
    public HandSide HandSide
    {
        get => _handSide;
        set => _handSide = value;
    }

    [SerializeField]
    internal TextMeshProUGUI MacAddressText;

    [SerializeField]
    internal TextMeshProUGUI BatteryLevelText;

    [SerializeField]
    internal Image ChargingImage;

    [SerializeField]
    internal GameObject InfoPanel;

    [SerializeField]
    internal TextMeshProUGUI CalibrationText;

    [SerializeField]
    [Min(0)]
    internal float CalibrationFadeOutTimeSeconds = 10;

    [SerializeField]
    internal RawImage ThumbStatusLight;

    [SerializeField]
    internal RawImage IndexStatusLight;

    [SerializeField]
    internal RawImage MiddleStatusLight;

    [SerializeField]
    internal Image HandImage;

    [SerializeField]
    internal Color OkColor = Color.green;

    [SerializeField]
    internal Color ErrorColor = Color.red;

    [SerializeField]
    internal Color DisconnectedColor = Color.gray;

    private WeArtTrackingCalibration calibrationTracker;
    private CalibrationStatus calibrationStatus = CalibrationStatus.IDLE;

    private bool _clientConnected = false;
    private bool _deviceConnected = false;
    private bool _sessionRunning = false;
    private DateTime _lastCalibrationCompletedTime = DateTime.MinValue;
    private DeviceStatusData currentStatus = new DeviceStatusData();

    private bool StandaloneAndroidActive = false;

    private void Init()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
            StandaloneAndroidActive = true;
#endif

        // Track middleware/devices status
        tracker._OnMiddlewareStatus.RemoveListener(OnDevicesStatus);
        tracker._OnMiddlewareStatus.AddListener(OnDevicesStatus);

        if (WeArtController.Instance is null)
            return;

        // Track calibration status
        calibrationTracker = WeArtController.Instance.gameObject.GetComponent<WeArtTrackingCalibration>();
        if (calibrationTracker != null)
        {
            calibrationTracker._OnCalibrationStart.RemoveListener(OnCalibrationStart);
            calibrationTracker._OnCalibrationStart.AddListener(OnCalibrationStart);

            calibrationTracker._OnCalibrationFinish.RemoveListener(OnCalibrationFinish);
            calibrationTracker._OnCalibrationFinish.AddListener(OnCalibrationFinish);
        }

        // Track connection status
        WeArtController.Instance.Client.OnConnectionStatusChanged -= OnConnectionChanged;
        WeArtController.Instance.Client.OnConnectionStatusChanged += OnConnectionChanged;
    }

    private void OnEnable()
    {
        Init();
    }

    private void OnConnectionChanged(bool sdkConnected)
    {
        _clientConnected = sdkConnected;
        if (!_clientConnected)
        {
            _deviceConnected = false;
            _sessionRunning = false;
            calibrationStatus = CalibrationStatus.IDLE;
        }
    }

    private void OnDevicesStatus(MiddlewareStatusData status)
    {
        _deviceConnected = false;
        _sessionRunning = _clientConnected && status.Status == MiddlewareStatus.RUNNING;

        if (status.Devices is null)
            return;
        foreach (DeviceStatusData device in status.Devices)
        {
            if (device.HandSide == _handSide)
            {
                _deviceConnected = true;
                currentStatus = device;
            }
        }
    }

    private void OnCalibrationStart(HandSide handSide)
    {
        if (handSide != _handSide)
            return;
        calibrationStatus = CalibrationStatus.Calibrating;
    }

    private void OnCalibrationFinish(HandSide handSide)
    {
        if (handSide != _handSide)
            return;
        calibrationStatus = CalibrationStatus.Running;
        _lastCalibrationCompletedTime = DateTime.Now;
    }

    private void Update()
    {
        UpdateHandStatus();
        UpdateInfoPanel();
    }

    private void UpdateInfoPanel()
    {
        InfoPanel.SetActive(_deviceConnected);
        if (StandaloneAndroidActive)
        {
            MacAddressText.text = currentStatus.FirmwareVersion;  
        }
        else
        {
            MacAddressText.text = currentStatus.MacAddress;
        }
        
        UpdateCalibrationStatus();
        UpdateBattery();
    }
    private void UpdateCalibrationStatus()
    {
        // Do not show calibration text if the session is not running (or the device is disconnected)
        if (!_sessionRunning)
        {
            CalibrationText.text = "";
            return;
        }

        switch (calibrationStatus)
        {
            case CalibrationStatus.IDLE: CalibrationText.text = ""; break;
            case CalibrationStatus.Calibrating: CalibrationText.text = "Calibrating..."; break;
            case CalibrationStatus.Running:
                {
                    // Stop showing calibration text after X seconds
                    bool timePassed = CalibrationFadeOutTimeSeconds > 0 && (DateTime.Now - _lastCalibrationCompletedTime).TotalSeconds > CalibrationFadeOutTimeSeconds;
                    CalibrationText.text = timePassed ? "" : "Calibrated!";
                }
                break;
        }
    }

    private void UpdateBattery()
    {
        BatteryLevelText.text = currentStatus.BatteryLevel.ToString() + " %";
        ChargingImage.enabled = currentStatus.Charging;
    }

    private void UpdateHandStatus()
    {
        HandImage.color = _deviceConnected ? Color.white : Color.gray;
        if (_deviceConnected)
        {
            foreach (var thimble in currentStatus.Thimbles)
            {
                Color color = ThimbleColor(thimble.Connected, thimble.StatusCode != 0);
                switch (thimble.Id)
                {
                    case ActuationPoint.Thumb: ThumbStatusLight.color = color; break;
                    case ActuationPoint.Index: IndexStatusLight.color = color; break;
                    case ActuationPoint.Middle: MiddleStatusLight.color = color; break;
                }
            }
        }
        else
        {
            ThumbStatusLight.color = DisconnectedColor;
            IndexStatusLight.color = DisconnectedColor;
            MiddleStatusLight.color = DisconnectedColor;
        }
    }

    private Color ThimbleColor(bool connected, bool ok)
    {
        return connected && connected ? (ok ? ErrorColor : OkColor) : DisconnectedColor;
    }
}
