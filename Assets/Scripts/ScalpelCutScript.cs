using UnityEngine;

public class ScalpelCutScript : MonoBehaviour
{
    [Header("Material (uses Custom/IndentShader)")]
    public Material indentMaterial;

    [Header("Optional: if not assigned, one will be created at runtime")]
    public Texture2D displacementTexture;

    [Header("Painting Settings")]
    [Tooltip("Radius of the 'cut' in texture pixels")]
    public float brushRadius = 10f;

    [Tooltip("How much we indent per paint stroke (0..1)")]
    public float indentAmount = 0.1f;

    private void Start()
    {
        // If the displacement map isn't assigned, create a blank one
        if (displacementTexture == null)
        {
            int texSize = 512;
            displacementTexture = new Texture2D(texSize, texSize, TextureFormat.RFloat, false);

            // Fill it with black (no indentation)
            Color[] pixels = new Color[texSize * texSize];
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = Color.black; 
            }
            displacementTexture.SetPixels(pixels);
            displacementTexture.Apply();

            // Assign it to the material
            if (indentMaterial != null)
            {
                indentMaterial.SetTexture("_DisplacementMap", displacementTexture);
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        // Only indent objects tagged "Indentable"
        if (collision.gameObject.CompareTag("Indentable"))
        {
            // For each contact point, we'll attempt a short Raycast 
            // to retrieve texture UV info from the mesh collider
            foreach (ContactPoint contact in collision.contacts)
            {
                // Slight offset along the normal to ensure we can raycast back onto the surface
                Vector3 offset = contact.normal * 0.01f;
                Ray ray = new Ray(contact.point + offset, -contact.normal);

                RaycastHit hit;
                // Check a short distance
                if (Physics.Raycast(ray, out hit, 0.05f))
                {
                    // If we hit the same object we're colliding with, paint
                    if (hit.collider.gameObject.CompareTag("Indentable"))
                    {
                        Vector2 uv = hit.textureCoord;
                        PaintIndent(uv);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Paints a circle (white) in the displacement texture at the given UV.
    /// White = stronger indent in the shader.
    /// </summary>
    private void PaintIndent(Vector2 uv)
    {
        if (displacementTexture == null) return;

        int xCenter = (int)(uv.x * displacementTexture.width);
        int yCenter = (int)(uv.y * displacementTexture.height);
        int radius = Mathf.RoundToInt(brushRadius);

        // Paint a simple circular falloff
        for (int y = yCenter - radius; y <= yCenter + radius; y++)
        {
            for (int x = xCenter - radius; x <= xCenter + radius; x++)
            {
                // Ensure we're within texture bounds
                if (x < 0 || x >= displacementTexture.width ||
                    y < 0 || y >= displacementTexture.height)
                {
                    continue;
                }

                // Check distance to center
                float dist = Vector2.Distance(new Vector2(x, y), new Vector2(xCenter, yCenter));
                if (dist <= radius)
                {
                    float falloff = 1f - (dist / radius); // 1 = center, 0 = edge
                    
                    // Current pixel color
                    Color oldColor = displacementTexture.GetPixel(x, y);
                    // Increase red channel, clamped to [0..1]
                    float newR = Mathf.Clamp01(oldColor.r + indentAmount * falloff);

                    // Write it back (only in R, we can keep G/B at 0)
                    displacementTexture.SetPixel(x, y, new Color(newR, 0f, 0f, 1f));
                }
            }
        }
        
        // Apply changes to the texture so the shader sees the new data
        displacementTexture.Apply();
    }
}
