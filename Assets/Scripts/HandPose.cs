using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Interaction.Toolkit;

public class HandPose : MonoBehaviour
{
    public HandType handType;
    public XRHandSkeletonDriver handSkeletonDriver;
    public GameObject[] fingerObjects;
    public HandData originlaPoseDataLeft;
    public HandData poseDataLeft;
    public HandData originlaPoseDataRight;
    public HandData poseDataRight;
    // Start is called before the first frame update
    void Start()
    {
        if (TryGetComponent(typeof(XRGrabInteractable), out Component grabInteractable))
        {
            XRGrabInteractable grabInteractableComponent = (XRGrabInteractable) grabInteractable;
            grabInteractableComponent.selectEntered.AddListener(SetPose);
            grabInteractableComponent.selectExited.AddListener(ResetPose);
        }


    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void CreatePose()
    {
        if (handType == HandType.Left)
        {
            CreatePose(ref poseDataLeft, ref originlaPoseDataLeft);
        }
        else
        {
            CreatePose(ref poseDataRight, ref originlaPoseDataRight);
        }
    }

    private void CreatePose(ref HandData poseData, ref HandData originalPoseData)
    {

        if (poseData == null)
        {
            poseData = new HandData();
            poseData.fingerPositions = new Vector3[fingerObjects.Length];
            poseData.fingerRotations = new Quaternion[fingerObjects.Length];
            poseData.fingerScales = new Vector3[fingerObjects.Length];
        }
        else if (poseData.fingerPositions == null || poseData.fingerPositions.Length != fingerObjects.Length)
        {
            poseData.fingerPositions = new Vector3[fingerObjects.Length];
            poseData.fingerRotations = new Quaternion[fingerObjects.Length];
            poseData.fingerScales = new Vector3[fingerObjects.Length];
        }

        for (int i = 0; i < fingerObjects.Length; i++)
        {
            poseData.fingerPositions[i] = fingerObjects[i].transform.localPosition;
            poseData.fingerRotations[i] = fingerObjects[i].transform.localRotation;
            poseData.fingerScales[i] = fingerObjects[i].transform.localScale;
        }
    }

    public void UsePose(HandPose handPose)
    {
        if (handType == HandType.Left)
        {
            UsePose(handPose, ref poseDataLeft, ref originlaPoseDataLeft);
        }
        else
        {
            UsePose(handPose, ref poseDataRight, ref originlaPoseDataRight);
        }
    }
    
    private void UsePose(HandPose handPose, ref HandData poseData, ref HandData originalPoseData)
    {
        
        if (poseData == null)
        {
            Debug.LogError("No Pose Data");
            return;
        }

        originalPoseData = new HandData();
        originalPoseData.fingerPositions = new Vector3[handPose.fingerObjects.Length];
        originalPoseData.fingerRotations = new Quaternion[handPose.fingerObjects.Length];
        originalPoseData.fingerScales = new Vector3[handPose.fingerObjects.Length];
        
        for (int i = 0; i < handPose.fingerObjects.Length; i++)
        {
            originalPoseData.fingerPositions[i] = handPose.fingerObjects[i].transform.localPosition;
            originalPoseData.fingerRotations[i] = handPose.fingerObjects[i].transform.localRotation;
            originalPoseData.fingerScales[i] = handPose.fingerObjects[i].transform.localScale;
        }
        
        for (int i = 0; i < handPose.fingerObjects.Length; i++)
        {
            handPose.fingerObjects[i].transform.localPosition = poseData.fingerPositions[i];
            handPose.fingerObjects[i].transform.localRotation = poseData.fingerRotations[i];
            handPose.fingerObjects[i].transform.localScale = poseData.fingerScales[i];
        }
    }

    public void SetPose(BaseInteractionEventArgs args)
    {
        Debug.Log("HAHAHAHHAH");
        if(args.interactorObject is XRDirectInteractor)
        {
            Debug.Log("HuHuHuHuHuH");
            XRDirectInteractor interactor = (XRDirectInteractor) args.interactorObject;
            HandPose handComponent = interactor.transform.parent.GetComponent<HandPose>();
            handComponent.handSkeletonDriver.UnsubscribeFromFingerTrackingEvents();
            UsePose(handComponent);
        } 
    }
    
    public void ResetPose(BaseInteractionEventArgs args)
    {
        if(args.interactorObject is XRDirectInteractor)
        {
            XRDirectInteractor interactor = (XRDirectInteractor) args.interactorObject;
            HandPose handComponent = interactor.transform.parent.GetComponent<HandPose>();
            ResetHand(handComponent);
            handComponent.handSkeletonDriver.SubscribeToFingerTrackingEvents();
        } 
    }

    public void ResetHand(HandPose handPose)
    {
        if (handPose.handType == HandType.Left)
        {

            if (originlaPoseDataLeft == null)
            {
                Debug.LogError("No Original Pose Data");
                return;
            }

            for (int i = 0; i < fingerObjects.Length; i++)
            {
                handPose.fingerObjects[i].transform.localPosition = originlaPoseDataLeft.fingerPositions[i];
                handPose.fingerObjects[i].transform.localRotation = originlaPoseDataLeft.fingerRotations[i];
                handPose.fingerObjects[i].transform.localScale = originlaPoseDataLeft.fingerScales[i];
            }
        }
        else
        {
            if (originlaPoseDataRight == null)
            {
                Debug.LogError("No Original Pose Data");
                return;
            }

            for (int i = 0; i < fingerObjects.Length; i++)
            {
                handPose.fingerObjects[i].transform.localPosition = originlaPoseDataRight.fingerPositions[i];
                handPose.fingerObjects[i].transform.localRotation = originlaPoseDataRight.fingerRotations[i];
                handPose.fingerObjects[i].transform.localScale = originlaPoseDataRight.fingerScales[i];
            }
        
        }
    }

}


[CustomEditor(typeof(HandPose))]
public class HandPoseEditor : Editor 
{
    public override void OnInspectorGUI() {
        HandPose hand = (HandPose)target;
        if (GUILayout.Button("Test"))
        {
          
            hand.CreatePose();
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }
    }
}

public enum HandType
{
    Left,
    Right
}