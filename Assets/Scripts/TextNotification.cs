using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace DefaultNamespace
{
    public class TextNotification : MonoBehaviour
    {
        [SerializeField] public TrackMainCamera canvas;
        [SerializeField] public TMP_Text textLabel;
        public static TextNotification _instance;

        public void Start()
        {
            if (_instance == null)
            {
                _instance = this;
            }
            else
            {
                Destroy(this);
            }
        }

        public IEnumerator ShowNotification(string textToDisplay, float displayTime)
        {
            canvas.SetPositionToCamera();
            textLabel.gameObject.SetActive(true);
            textLabel.text = textToDisplay;
            canvas.StartTracking();
            yield return new WaitForSeconds(displayTime);
            textLabel.gameObject.SetActive(false);
            canvas.StopTracking();
        }
    }
}