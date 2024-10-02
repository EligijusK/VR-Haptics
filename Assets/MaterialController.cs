using System;
using UnityEngine;

public class MaterialController : MonoBehaviour
{
    private void Start()
    {
        DisableFirstMaterial();
    }

    public void DisableFirstMaterial()
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            Material[] materials = meshRenderer.materials;

            if (materials.Length > 0)
            {
                Material[] newMaterials = new Material[materials.Length - 1];
                for (int i = 1; i < materials.Length; i++)
                {
                    newMaterials[i - 1] = materials[i];
                }
                meshRenderer.materials = newMaterials;
            }
            else
            {
                Debug.LogWarning("No materials to disable.");
            }
        }
        else
        {
            Debug.LogError("MeshRenderer not found.");
        }
    }

    public void EnableFirstMaterial(Material firstMaterial)
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null && firstMaterial != null)
        {
            Material[] materials = meshRenderer.materials;
            Material[] newMaterials = new Material[materials.Length + 1];

            newMaterials[0] = firstMaterial;

            for (int i = 0; i < materials.Length; i++)
            {
                newMaterials[i + 1] = materials[i];
            }

            meshRenderer.materials = newMaterials;
        }
        else
        {
            Debug.LogError("MeshRenderer or firstMaterial is null.");
        }
    }
}
