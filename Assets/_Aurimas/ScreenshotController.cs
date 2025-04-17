using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ScreenshotController : MonoBehaviour
{
    public Image displayImage;
    public Button nextButton;
    public Button prevButton;
    
    private List<Texture2D> screenshots;
    private int currentIndex = 0;
    void Start()
    {
        screenshots = ScoreManager.wrongAnswerScreenshots;
        Debug.Log($"[ScreenshotController] {screenshots.Count} screenshots available");
        screenshots = ScoreManager.wrongAnswerScreenshots;

        // Hook up button callbacks
        nextButton.onClick.AddListener(ShowNext);
        if (prevButton != null)
            prevButton.onClick.AddListener(ShowPrevious);
        
        if (screenshots.Count > 0)
            UpdateDisplay();
    }

    
    void UpdateDisplay()
    {
        // Create a sprite from the current Texture2D
        Texture2D tex = screenshots[currentIndex];
        Sprite spr = Sprite.Create(
            tex,
            new Rect(0, 0, tex.width, tex.height),
            new Vector2(0.5f, 0.5f)
        );
        displayImage.sprite = spr;

        // Optionally disable prev/next at ends
        if (prevButton != null)
            prevButton.interactable = (currentIndex > 0);
        nextButton.interactable = (currentIndex < screenshots.Count - 1);
    }
    
    public void ShowNext()
    {
        if (currentIndex < screenshots.Count - 1)
        {
            currentIndex++;
            UpdateDisplay();
        }
    }

    public void ShowPrevious()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            UpdateDisplay();
        }
    }
}
