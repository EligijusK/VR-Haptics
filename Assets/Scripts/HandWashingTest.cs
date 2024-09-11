using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.XR.Interaction.Toolkit;

namespace DefaultNamespace
{
    public class HandWashingTest : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private GameObject videoFeed; 
        [SerializeField] OrderingInLine checkOrderInList;
        [SerializeField] VideoClipController videoClipController;
        [SerializeField] VideoPlayer videoController;
        [SerializeField] VideoClipController mistakeVideoClipController;
        [SerializeField] List<XRGrabInteractable> interactables;
        [SerializeField] string wrongText = "";
        [SerializeField] int allowedMistakes = 3;
        [SerializeField] bool playVideoIfMistake = true;
        [SerializeField] bool playVideoIfNotMistakes = true;
        
        int mistakes = 0;
        private bool incorrectOnce = false;
        
        
        public void CheckIfElementIsInCorrectPlace(int index)
        {
            if (mistakes < allowedMistakes)
            {
                bool isCorrect = checkOrderInList.CheckOrderInList(index);
                if (isCorrect && playVideoIfNotMistakes)
                {
                    // VideoClip videoClip = videoClipController.GetVideoClip(index);
                    // videoController.Stop();
                    // videoController.frame = 0;
                    // videoController.clip = videoClip;
                    // videoController.Play();
                }
                if(!isCorrect && !incorrectOnce)
                {
                    mistakes++;
                    StartCoroutine(TextNotification._instance.ShowNotification("Mistakes: " + mistakes + " / " + allowedMistakes, 1f));
                }
            }
            if (playVideoIfMistake && mistakes >= allowedMistakes && !incorrectOnce)
            {
                StartCoroutine(WrongText());
                incorrectOnce = true;
            }
        }

        private void Update()
        {
            if (incorrectOnce && !videoController.isPlaying)
            {
                incorrectOnce = false;
                mistakes = 0;
                StartCoroutine(TextNotification._instance.ShowNotification("Try one more time", 1f));
            }
        }
        

        public void StartFadingText()
        {
            text.gameObject.SetActive(true);
            text.CrossFadeAlpha(1f, 0f, false);
            StartCoroutine(FadeText());
        }
        
        private IEnumerator FadeText()
        {
            text.CrossFadeAlpha(0f, 10f, false);
            yield return new WaitForSeconds(11f);
            text.gameObject.SetActive(false);
            
        }
        
        private IEnumerator WrongText()
        {
            for (int i = 0; i < interactables.Count; i++)
            {
                interactables[i].enabled = false;
            }
            text.gameObject.SetActive(true);
            text.text = wrongText;
            text.CrossFadeAlpha(1f, 2f, false);
            yield return new WaitForSeconds(11f);
            text.CrossFadeAlpha(0f, 2f, false);
            yield return new WaitForSeconds(2f);
            text.gameObject.SetActive(false);
            videoController.Stop();
            videoController.frame = 0;
            videoFeed.SetActive(true);
            videoController.clip = mistakeVideoClipController.GetVideoClip(-1);
            videoController.Play();
            videoController.loopPointReached += vp =>
            {
                videoFeed.SetActive(false);
                videoController.Stop();
                videoController.frame = 0;
                incorrectOnce = false;
                mistakes = 0;
                for (int i = 0; i < interactables.Count; i++)
                {
                    interactables[i].enabled = true;
                }
            };
        }
    }
}