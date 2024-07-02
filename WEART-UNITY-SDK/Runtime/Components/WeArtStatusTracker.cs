using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.PlayerLoop;
using WeArt.Core;
using WeArt.Messages;

namespace WeArt.Components
{
    public struct MiddlewareStatusData
    {
        public MiddlewareStatus Status { get; set; }
        public string Version { get; set; }
        public int StatusCode { get; set; }
        public string ErrorDesc { get; set; }
        public bool ActuationsEnabled { get; set; }
        public List<DeviceStatusData> Devices { get; set; }

        public static MiddlewareStatusData Empty => new MiddlewareStatusData()
        {
            Status = MiddlewareStatus.DISCONNECTED,
            ActuationsEnabled = false,
            StatusCode = 0,
            ErrorDesc = "",
            Version = "",
            Devices = new List<DeviceStatusData>(),
        };

        
    }

    

    /// <summary>
    /// Receives and notifies of middleware and connected devices status changes
    /// </summary>
    public class WeArtStatusTracker : MonoBehaviour
    {
        [System.Serializable]
        public class StatusTrackingEvent : UnityEvent<MiddlewareStatusData> { }
        
        /// <summary>
        /// Event fired when a the middleware status changes
        /// </summary>
        [SerializeField]
        internal StatusTrackingEvent _OnMiddlewareStatus;

        #region STATUS_TRACKING_RUNTIME_EDITOR
        public MiddlewareStatus Status { get; private set; } = MiddlewareStatus.DISCONNECTED;
        public string Version { get; private set; } = "";
        public int StatusCode { get; private set; } = 0;
        public string ErrorDesc { get; private set; } = "";
        public bool ActuationsEnabled { get; private set; } = false;
        public List<DeviceStatusData> Devices { get; private set; } = new List<DeviceStatusData>();
        #endregion

        private MiddlewareStatusData _currentMiddlewareStatus;
        private MiddlewareStatusData _oldMiddlewareStatus;
        private bool _newStatus_Received = false;

        /// <summary>
        /// Delegate for Connected Devices
        /// </summary>
        /// <typeparam name="ConnectedDevices"></typeparam>
        /// <param name="e"></param>
        public delegate void dConnectedDevices<ConnectedDevices>(ConnectedDevices e);
        /// <summary>
        /// Event for handling connected devices from middleware
        /// </summary>
        public static event dConnectedDevices<ConnectedDevices> ConnectedDevicesReady;

        /// <summary>
        /// Current number of connected devices on Middleware
        /// </summary>
        private int CurrentConnectedDevices = 0;

        private void Update()
        {
            if (_newStatus_Received)
            {
                _OnMiddlewareStatus?.Invoke(_currentMiddlewareStatus);
                UpdateStatusFields();
                _newStatus_Received = false;
            }
        }

        private void Init()
        {
            _newStatus_Received = false;

            if (WeArtController.Instance is null)
                return;

            var client = WeArtController.Instance.Client;
            client.OnConnectionStatusChanged -= OnConnectionChanged;
            client.OnConnectionStatusChanged += OnConnectionChanged;
            client.OnMessage -= OnMessageReceived;
            client.OnMessage += OnMessageReceived;
        }

        private void OnEnable()
        {
            Init();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (gameObject.scene.IsValid())
                Init();
        }
#endif

        internal void OnConnectionChanged(bool connected)
        {
            if (connected)
                return;

            _currentMiddlewareStatus = MiddlewareStatusData.Empty;
            _newStatus_Received = false;
        }

        private void OnMessageReceived(WeArtClient.MessageType type, IWeArtMessage message)
        {
            if (message is MiddlewareStatusMessage mwStatusMessage)
            {
                _currentMiddlewareStatus.Status = mwStatusMessage.Status;
                _currentMiddlewareStatus.Version = mwStatusMessage.Version;
                _currentMiddlewareStatus.ActuationsEnabled = mwStatusMessage.ActuationsEnabled;
                _currentMiddlewareStatus.StatusCode = mwStatusMessage.StatusCode;
                _currentMiddlewareStatus.ErrorDesc = mwStatusMessage.ErrorDesc;
                _newStatus_Received = true;
            }
                
            if (message is DevicesStatusMessage devicesStatusMessage)
            {
                _currentMiddlewareStatus.Devices = devicesStatusMessage.Devices.ToList();
                _newStatus_Received = true;
            }
        }

        private void UpdateStatusFields()
        {
            Status = _currentMiddlewareStatus.Status;
            Version =  _currentMiddlewareStatus.Version;
            StatusCode =  _currentMiddlewareStatus.StatusCode;
            ErrorDesc =  _currentMiddlewareStatus.ErrorDesc;
            ActuationsEnabled =  _currentMiddlewareStatus.ActuationsEnabled;
            Devices =  _currentMiddlewareStatus.Devices;
            AskForMiddlewareStatusIfNeeded();
            GenerateEventConnectedDevices(Devices, Status == MiddlewareStatus.RUNNING ? true : false);
        }

        /// <summary>
        /// Ask For Middleware and Devices Status if needed (when passing from IDLE to RUNNING)
        /// </summary>
        private void AskForMiddlewareStatusIfNeeded()
        {
            if (Status == MiddlewareStatus.RUNNING && Status != _oldMiddlewareStatus.Status)
            {
                WeArtController.Instance.Client.SendMessage(new GetMiddlewareStatusMessage());
                WeArtController.Instance.Client.SendMessage(new GetDevicesStatusMessage());
            }
            _oldMiddlewareStatus = _currentMiddlewareStatus;
        }

        /// <summary>
        /// Shows/Hides this panel
        /// </summary>
        public void ShowHidePanel()
        {
            gameObject.SetActive(!isActiveAndEnabled);
        }

        /// <summary>
        /// Method to generate event on Connected Devices change
        /// </summary>
        /// <param name="devices"></param>
        private void GenerateEventConnectedDevices(List<DeviceStatusData> devices, bool middlewareRunning)
        {
            // If number of current connected devices changes, we notify WeArtController
            if (devices != null)
            {
                CurrentConnectedDevices = devices.Count;

                ConnectedDevices connectedDevices = new ConnectedDevices(devices, middlewareRunning);
                ConnectedDevicesReady?.Invoke(connectedDevices);  
            }
        }           
    }
}