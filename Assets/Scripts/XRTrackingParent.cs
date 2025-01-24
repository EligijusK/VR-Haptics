using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRTrackingParent : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private int _newLayer;
    private Transform oldParent;
    private int oldLayer;
    
    public void ChangeParent(SelectEnterEventArgs args)
    {
        
        XRGrabInteractable grabInteractable = args.interactableObject.transform.gameObject
                                              .GetComponent<XRGrabInteractable>();
        if (grabInteractable == null)
        {
            return; 
        }

        Transform grabbedTransform = args.interactableObject.transform;

        oldParent = grabbedTransform.parent;
        oldLayer  = grabbedTransform.gameObject.layer;
        
        grabbedTransform.SetParent(target);

        SetLayerRecursively(grabbedTransform, _newLayer);
    }

    public void RestoreParent(SelectExitEventArgs args)
    {
        XRGrabInteractable grabInteractable = args.interactableObject.transform.gameObject
                                              .GetComponent<XRGrabInteractable>();
        if (grabInteractable == null)
        {
            return;
        }

        Transform grabbedTransform = args.interactableObject.transform;

        grabbedTransform.SetParent(oldParent);

        SetLayerRecursively(grabbedTransform, oldLayer);
    }

    private void SetLayerRecursively(Transform obj, int newLayer)
    {
        obj.gameObject.layer = newLayer;
        for (int i = 0; i < obj.childCount; i++)
        {
            SetLayerRecursively(obj.GetChild(i), newLayer);
        }
    }
}
