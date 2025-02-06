using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineColorChange : MonoBehaviour
{
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] Color activeColor;
    [SerializeField] bool activateAtStart;
    Material lineMaterial;
    Color originalColor;
    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineMaterial = lineRenderer.material;
        originalColor = lineMaterial.color;
        if (activateAtStart)
        {
            ActivateColor();
        }
    }

    public void ActivateColor()
    {
        lineMaterial.color = activeColor;
    }

    public void DeactivateColor()
    {
        lineMaterial.color = originalColor;
    }
}
