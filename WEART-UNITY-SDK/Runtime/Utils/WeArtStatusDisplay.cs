using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WeArt.Components;
using WeArt.Core;
using WeArt.Messages;

namespace WeArt.Utils
{

    public class WeArtStatusDisplay : MonoBehaviour
    {
        [SerializeField]
        internal WeArtStatusTracker tracker;

        [SerializeField]
        internal TextMeshProUGUI MiddlewareStatusText;

        [SerializeField]
        internal TextMeshProUGUI MiddlewareStatusCodeText;

        [SerializeField]
        internal TextMeshProUGUI MiddlewareVersionText;

        [SerializeField]
        internal TextMeshProUGUI ActuationsText;

        [SerializeField]
        internal TextMeshProUGUI ErrorDescriptionText;

        [SerializeField]
        internal GameObject ErrorDescriptionPanel;
        
        private bool Connected = false;
        private MiddlewareStatusData currentStatus = MiddlewareStatusData.Empty;

        private bool StandaloneAndroidActive = false;

        private void Init()
        {
            tracker._OnMiddlewareStatus.RemoveListener(OnMiddlewareStatus);
            tracker._OnMiddlewareStatus.AddListener(OnMiddlewareStatus);

            if (WeArtController.Instance is null)
                return;

            WeArtController.Instance.Client.OnConnectionStatusChanged += OnConnectionChanged;

            // Force Connected value to true when targeting ANDROID architecture
#if UNITY_ANDROID && !UNITY_EDITOR
            WriteInitialInfoOnUI();
            StandaloneAndroidActive = true;
            Connected = true;
#endif
        }

        private void OnEnable()
        {
            Init();
        }

        public void OnConnectionChanged(bool connected)
        {
            Connected = connected;
        }

        public void WriteInitialInfoOnUI()
        {
            currentStatus.Version = WeArtConstants.WEART_SDK_VERSION;
            MiddlewareVersionText.text = currentStatus.Version;
        }

        public void OnMiddlewareStatus(MiddlewareStatusData newStatusData)
        {
            currentStatus = newStatusData;
        }

        private void Update()
        {
            Connected = StandaloneAndroidActive ? true : Connected;
            bool canShowFields = true;

            MiddlewareStatusText.text = Connected ? currentStatus.Status.ToString() : "DISCONNECTED";
            MiddlewareStatusText.color = MiddlewareStatusColor(currentStatus.Status);
            
            if (StandaloneAndroidActive)
                MiddlewareVersionText.text = WeArtConstants.WEART_SDK_VERSION;            
            else
                MiddlewareVersionText.text = currentStatus.Version;


            bool isOk = currentStatus.StatusCode == 0;

            string statusCodeText = canShowFields ? (isOk ? "OK" : "[" + currentStatus.StatusCode + "] "): "-";
            MiddlewareStatusCodeText.text = statusCodeText;

            MiddlewareStatusCodeText.color = canShowFields ? (isOk ? Color.green : Color.red) : Color.white;

            ErrorDescriptionPanel.SetActive(canShowFields & !isOk);

            if (!isOk)
            {
                string fullErrorDescription = "[" + currentStatus.StatusCode + "] " + currentStatus.ErrorDesc;
                ErrorDescriptionText.text = fullErrorDescription;
            }
            else
            {
                ErrorDescriptionText.text = "-";
            }

            if (StandaloneAndroidActive)
            {
                ActuationsText.text = canShowFields ? "YES" : "-";
                ActuationsText.color = canShowFields ? Color.green : Color.red;
            }
            else
            {
                ActuationsText.text = canShowFields ? (currentStatus.ActuationsEnabled ? "YES" : "NO") : "-";
                ActuationsText.color = canShowFields ? (currentStatus.ActuationsEnabled ? Color.green : Color.red) : Color.white;
            }           
        }

        private Color MiddlewareStatusColor(MiddlewareStatus status)
        {
            bool isOk = Connected && status == MiddlewareStatus.RUNNING;
            bool isWarning = status != MiddlewareStatus.RUNNING && status != MiddlewareStatus.DISCONNECTED;

            return isOk ? Color.green : (isWarning ? Color.yellow : Color.red);
        }
    }
}