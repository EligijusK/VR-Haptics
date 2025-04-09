using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayScreenshots : MonoBehaviour
{
    // Reference to a prefab with an Image component.
    public GameObject imagePrefab;
    // The parent transform where the image objects will be added.
    public Transform contentParent;

    private void Start()
    {
        // Loop through each captured screenshot and create a UI image.
        foreach (Texture2D tex in ScoreManager.wrongAnswerScreenshots)
        {
            GameObject newImageObj = Instantiate(imagePrefab, contentParent);
            Image imageComp = newImageObj.GetComponent<Image>();

            if (imageComp != null)
            {
                // Create a sprite out of the texture.
                Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                imageComp.sprite = sprite;
            }
        }
    }
}
