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
        [SerializeField] List<string> correctTexts;
        [SerializeField] string continueText;
        [SerializeField] string wrongText = "";
        [SerializeField] int allowedMistakes = 3;
        [SerializeField] bool playVideoIfMistake = true;
        [SerializeField] bool playVideoIfNotMistakes = true;
        
        int mistakes = 0;
        private bool incorrectOnce = false;
        private bool correctOnce = false;
        
        
        public void CheckIfElementIsInCorrectPlace(int index)
        {
            if (mistakes < allowedMistakes)
            {
                bool isCorrect = checkOrderInList.CheckOrderInList(index);
                Debug.Log( "Element is in place: " + isCorrect);
                if (isCorrect && playVideoIfNotMistakes && !correctOnce)
                {
                    StartCoroutine(CorrectText(index));
                    Debug.Log("correct");
                    // VideoClip videoClip = videoClipController.GetVideoClip(index);
                    // videoController.Stop();
                    // videoController.frame = 0;
                    // videoController.clip = videoClip;
                    // videoController.Play();
                }
                if(!isCorrect && !incorrectOnce && playVideoIfMistake)
                {
                    mistakes++;
                    StartCoroutine(TextNotification._instance.ShowNotification("Klaidos: " + mistakes + " / " + allowedMistakes, 1f));
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
            if (incorrectOnce && !videoController.isPlaying && playVideoIfMistake)
            {
                incorrectOnce = false;
                mistakes = 0;
                StartCoroutine(TextNotification._instance.ShowNotification("Bandykite dar kartą.", 1f));
                ScoreManager.UpdateScore(-1);
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
        
        private IEnumerator CorrectText(int index)
        {
            correctOnce = true;
            for (int i = 0; i < interactables.Count; i++)
            {
                interactables[i].enabled = false;
            }
            text.gameObject.SetActive(true);
            //text.text = correctTexts[index];
            text.CrossFadeAlpha(1f, 2f, false);
            yield return new WaitForSeconds(11f);
            text.CrossFadeAlpha(0f, 2f, false);
            yield return new WaitForSeconds(2f);
            text.gameObject.SetActive(false);
            if (videoClipController.GetVideoClip(index) != null)
            {
                videoController.Stop();
                videoController.frame = 0;
                videoFeed.SetActive(true);
                videoController.clip = videoClipController.GetVideoClip(index);
                videoController.Play();
                videoController.loopPointReached += vp =>
                {
                    videoFeed.SetActive(false);
                    videoController.Stop();
                    videoController.frame = 0;
                    correctOnce = false;
                    mistakes = 0;
                    for (int i = 0; i < interactables.Count; i++)
                    {
                        interactables[i].enabled = true;
                    }
                };
            }
            else
            {
                text.gameObject.SetActive(true);
                text.text = continueText;
                text.CrossFadeAlpha(1f, 2f, false);
                yield return new WaitForSeconds(11f);
                text.CrossFadeAlpha(0f, 2f, false);
                yield return new WaitForSeconds(2f);
                text.gameObject.SetActive(false);
                correctOnce = false;
                mistakes = 0;
                for (int i = 0; i < interactables.Count; i++)
                {
                    interactables[i].enabled = true;
                }
            }
           
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