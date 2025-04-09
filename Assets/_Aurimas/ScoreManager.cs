using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ScoreManager : MonoBehaviour
{
    public static int score = 0;
    public static int expectedOrder = 1;
    
    public TMP_Text scoreText;
    
    public static List<Texture2D> wrongAnswerScreenshots = new List<Texture2D>();
    public static void UpdateScore(int value)
        {
            score += value;
            if(score < 0)
            {
                score = 0;
            }
            Debug.Log("Current Score: " + score);
        }
    
        // Call this after a successful pickup to update the expected order.
        public static void AdvanceOrder()
        {
            expectedOrder++;
            Debug.Log("Next expected order: " + expectedOrder);
        }
        public void HandleWrongAnswer()
        {
            UpdateScore(-1);  // Deduct points (will not fall below 0)
            // Start a coroutine to capture the screenshot.
            StartCoroutine(CaptureScreenshot());
        }

        // Capture the screenshot at the end of the frame.
        private IEnumerator CaptureScreenshot()
        {
            yield return new WaitForEndOfFrame();
            Texture2D screenshot = ScreenCapture.CaptureScreenshotAsTexture();
            wrongAnswerScreenshots.Add(screenshot);
            Debug.Log("Screenshot captured for wrong answer.");
        }
    
        // Update UI every frame (if needed).
        void Update()
        {
            if(scoreText != null)
                scoreText.text = "Score: " + score;
        }
}
