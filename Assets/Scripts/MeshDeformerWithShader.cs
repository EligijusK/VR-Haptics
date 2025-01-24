using UnityEngine;

[RequireComponent(typeof(Collider))]
public class MeshDeformerWithShader : MonoBehaviour
{
    [Header("Shader / Material Setup")]
    public Material skinMaterial; // Assign the CutShaderWithMask material in the Inspector
    public Texture2D cutMask;     // We'll create this at runtime if not assigned

    [Header("Scalpel / Cutting Setup")]
    public Transform scalpel;     // The scalpel transform
    public float paintRadius = 0.01f;      // UV radius for painting, can tweak
    public float worldRaycastDistance = 1.0f; // how far we raycast from the scalpel tip

    private void Start()
    {
        // If no texture is assigned in the Inspector, create a 512x512 black texture at runtime.
        if (cutMask == null)
        {
            // Create a new 512x512 texture in memory, RGBA32 is typical
            cutMask = new Texture2D(512, 512, TextureFormat.RGBA32, false);

            // Fill it entirely black
            Color[] pixels = new Color[512 * 512];
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = Color.black;
            }
            cutMask.SetPixels(pixels);
            cutMask.Apply();
        }

        // Make sure our shader gets the cutMask
        if (skinMaterial != null)
        {
            skinMaterial.SetTexture("_CutMask", cutMask);
        }
    }

    private void Update()
    {
        // Continuously pass scalpel's position to the shader if needed
        if (skinMaterial != null && scalpel != null)
        {
            skinMaterial.SetVector("_ScalpelPos", scalpel.position);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // If the object entering the trigger is the scalpel...
        if (other.CompareTag("Scalpel"))
        {
            // We'll do a forward-facing raycast from the scalpel tip to find UV on the mesh
            Ray ray = new Ray(scalpel.position, scalpel.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, worldRaycastDistance))
            {
                // If we actually hit THIS mesh's collider
                if (hit.collider.gameObject == this.gameObject)
                {
                    Vector2 hitUV = hit.textureCoord;
                    // Paint into the cutMask at that UV
                    PaintCut(hitUV);
                }
            }
        }
    }

    // Paint a small circle of white onto the cutMask to indicate a "cut"
    private void PaintCut(Vector2 uv)
    {
        if (cutMask == null) return;

        int texWidth = cutMask.width;
        int texHeight = cutMask.height;

        // Convert UV (0..1) to pixel coordinates in the texture
        int centerX = (int)(uv.x * texWidth);
        int centerY = (int)(uv.y * texHeight);

        // Determine how many pixels we'll paint in each direction
        int radiusPixels = Mathf.RoundToInt(paintRadius * texWidth);

        // Paint a circle of white in the texture
        for (int y = -radiusPixels; y <= radiusPixels; y++)
        {
            for (int x = -radiusPixels; x <= radiusPixels; x++)
            {
                int px = centerX + x;
                int py = centerY + y;

                // Check bounds
                if (px < 0 || px >= texWidth || py < 0 || py >= texHeight)
                    continue;

                // Check distance to center
                float dist = Mathf.Sqrt(x * x + y * y);
                if (dist <= radiusPixels)
                {
                    // Set pixel to white (cut)
                    // You could blend or use gradient if you want a soft edge
                    cutMask.SetPixel(px, py, Color.white);
                }
            }
        }

        // Apply the changes so the GPU sees the updated texture
        cutMask.Apply();
    }
}
