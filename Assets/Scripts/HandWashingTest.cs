using System;
using UnityEngine;
using UnityEngine.Video;

namespace DefaultNamespace
{
    public class HandWashingTest : MonoBehaviour
    {
        [SerializeField] OrderingInLine checkOrderInList;
        [SerializeField] VideoClipController videoClipController;
        [SerializeField] VideoPlayer videoController;
        [SerializeField] VideoClipController mistakeVideoClipController;
        [SerializeField] int allowedMistakes = 3;
        [SerializeField] bool playVideoIfMistake = true;
        int mistakes = 0;
        private bool incorrectOnce = false;
        
        
        public void CheckIfElementIsInCorrectPlace(int index)
        {
            if (mistakes < allowedMistakes)
            {
                bool isCorrect = checkOrderInList.CheckOrderInList(index);
                if (isCorrect && !playVideoIfMistake)
                {
                    VideoClip videoClip = videoClipController.GetVideoClip(index);
                    videoController.Stop();
                    videoController.frame = 0;
                    videoController.clip = videoClip;
                    videoController.Play();
                }
                else
                {
                    mistakes++;
                    StartCoroutine(TextNotification._instance.ShowNotification("Mistakes: " + mistakes + " / " + allowedMistakes, 1f));
                }
            }
            if (playVideoIfMistake && mistakes >= allowedMistakes && !incorrectOnce)
            {
                videoController.Stop();
                videoController.frame = 0;
                videoController.clip = mistakeVideoClipController.GetVideoClip(-1);
                videoController.Play();
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
    }
}