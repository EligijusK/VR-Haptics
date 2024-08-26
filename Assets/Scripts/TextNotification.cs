using System.Collections;
using TMPro;
using UnityEngine;

namespace DefaultNamespace
{
    public class TextNotification : MonoBehaviour
    {
        [SerializeField] public TrackMainCamera canvas;
        [SerializeField] public TMP_Text textLabel;
        [SerializeField] private float fadeDuration = 0.5f;
        public static TextNotification _instance;
        
        private void Start()
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

        public IEnumerator ShowNotification(string textToDisplay, float displayTime, Color? textColor = null)
        {
            Color color = textColor ?? Color.white;
            //textLabel.color = new Color(color.r, color.g, color.b, 0.1f);
            textLabel.CrossFadeAlpha(0f, 0f, false);
            textLabel.text = textToDisplay;
            textLabel.gameObject.SetActive(true);
            canvas.SetPositionToCamera();
            canvas.StartTracking();
            textLabel.CrossFadeAlpha(1.0f, fadeDuration, false);
            yield return new WaitForSeconds(fadeDuration);
            yield return new WaitForSeconds(displayTime);
            textLabel.CrossFadeAlpha(0.1f, fadeDuration, false);
            yield return new WaitForSeconds(fadeDuration);
            textLabel.gameObject.SetActive(false);
            canvas.StopTracking();
        }
    }
}