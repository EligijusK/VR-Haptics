using UnityEngine;

public class WoundCutTrigger : MonoBehaviour
{
    // Reference the script which updates blend shapes 
    public AccumulateBlendShapesWhileCutting cuttingScript;
    public string scalpelTag = "Scalpel";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(scalpelTag))
        {
            // Start cutting
            cuttingScript.SetCuttingActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(scalpelTag))
        {
            // Stop cutting
            cuttingScript.SetCuttingActive(false);
        }
    }
}