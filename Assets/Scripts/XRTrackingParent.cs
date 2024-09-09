using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRTrackingParent : MonoBehaviour
{
    [SerializeField] Transform target;

    private int layer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeParent(SelectEnterEventArgs args)
    {
        args.interactableObject.transform.SetParent(target);
        layer = args.interactableObject.transform.gameObject.layer;
        args.interactableObject.transform.gameObject.layer = 7;
    }
    
    public void RestoreParent(SelectExitEventArgs args)
    {
        args.interactableObject.transform.gameObject.layer = layer;
    }
}
