using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class HandPose : MonoBehaviour
{
    
    public GameObject[] fingerObjects;
    public HandData originlaPoseData;
    public HandData poseData;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreatePose()
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
            poseData.fingerPositions[i] = fingerObjects[i].transform.position;
            poseData.fingerRotations[i] = fingerObjects[i].transform.rotation;
            poseData.fingerScales[i] = fingerObjects[i].transform.lossyScale;
        }
    }
    
    public void UsePose(HandPose handPose)
    {
        if (poseData == null)
        {
            Debug.LogError("No Pose Data");
            return;
        }
        
        for (int i = 0; i < handPose.fingerObjects.Length; i++)
        {
            originlaPoseData.fingerPositions[i] = handPose.fingerObjects[i].transform.position;
            originlaPoseData.fingerRotations[i] = handPose.fingerObjects[i].transform.rotation;
            originlaPoseData.fingerScales[i] = handPose.fingerObjects[i].transform.lossyScale;
        }
        
        for (int i = 0; i < fingerObjects.Length; i++)
        {
            handPose.fingerObjects[i].transform.position = poseData.fingerPositions[i];
            handPose.fingerObjects[i].transform.rotation = poseData.fingerRotations[i];
            handPose.fingerObjects[i].transform.localScale = poseData.fingerScales[i];
        }
    }
    
    public void ResetPose(HandPose handPose)
    {
        if (originlaPoseData == null)
        {
            Debug.LogError("No Original Pose Data");
            return;
        }
        for (int i = 0; i < fingerObjects.Length; i++)
        {
            handPose.fingerObjects[i].transform.position = originlaPoseData.fingerPositions[i];
            handPose.fingerObjects[i].transform.rotation = originlaPoseData.fingerRotations[i];
            handPose.fingerObjects[i].transform.localScale = originlaPoseData.fingerScales[i];
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