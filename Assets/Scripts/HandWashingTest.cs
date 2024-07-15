using UnityEngine;
using UnityEngine.Video;

namespace DefaultNamespace
{
    public class HandWashingTest : MonoBehaviour
    {
        [SerializeField] OrderingInLine checkOrderInList;
        [SerializeField] VideoClipController videoClipController;
        [SerializeField] VideoPlayer videoController;
        [SerializeField] VideoClip mistakeVideoClip;
        [SerializeField] int allowedMistakes = 3;
        int mistakes = 0;
        
        public void CheckIfElementIsInCorrectPlace(int index)
        {
            if (mistakes < allowedMistakes)
            {
                bool isCorrect = checkOrderInList.CheckOrderInList(index);
                if (isCorrect)
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
                }
            }
            else
            {
                videoController.Stop();
                videoController.frame = 0;
                videoController.clip = mistakeVideoClip;
                videoController.Play();
            }
        }
    }
}