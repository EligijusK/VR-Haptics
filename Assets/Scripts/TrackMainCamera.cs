using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackMainCamera : MonoBehaviour
{
    [SerializeField] Vector3 cameraAngle;
    [SerializeField] float offset;
    [SerializeField] Material _material;
    bool isTracking = false;
    bool backToStart = false;
    Vector3 startingPosition;
    Quaternion startingRotation;
    Camera mainCamera;
    // Start is called before the first frame update
    void Awake()
    {
        startingPosition = transform.position;
        startingRotation = transform.rotation;
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (isTracking)
        {
            transform.position = Vector3.Slerp(transform.position, mainCamera.transform.position + (mainCamera.transform.forward * offset), Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(mainCamera.transform.rotation.eulerAngles + cameraAngle), Time.deltaTime);
            // transform.LookAt(mainCamera.transform);
        }
        else if (Vector3.Distance(transform.position, startingPosition) > 0.01 &&  backToStart)
        {
            transform.position = Vector3.Slerp(transform.position, startingPosition, Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, startingRotation, Time.deltaTime);
        }
        else if (backToStart)
        {
            backToStart = false;
        }
    }
    
    public void StartTracking()
    {
        _material.renderQueue = 3001;
        backToStart = false;
        isTracking = true;
        
    }
    
    public void StopTracking()
    {
        _material.renderQueue = 1;
        isTracking = false;
        backToStart = true;
    }

    public void SetPositionToCamera()
    {
        transform.position = Vector3.Slerp(transform.position, mainCamera.transform.position + (mainCamera.transform.forward * offset), 1f);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(mainCamera.transform.rotation.eulerAngles + cameraAngle), 1f);
    }

    private void OnDestroy()
    {
        StopTracking();
    }
}
