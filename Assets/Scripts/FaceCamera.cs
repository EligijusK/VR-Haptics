using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    [SerializeField] Vector3 cameraAngleOffset;
    [SerializeField] Material _material;
    bool isTracking = false;
    bool backToStart = false;
    Vector3 startingPosition;
    Quaternion startingRotation;
    Camera mainCamera;

    void Awake()
    {
        startingPosition = transform.position;
        startingRotation = transform.rotation;
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (isTracking)
        {
            Vector3 directionToCamera = mainCamera.transform.position - transform.position;
            directionToCamera.y = 0; 

            Quaternion targetRotation = Quaternion.LookRotation(-directionToCamera);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation * Quaternion.Euler(cameraAngleOffset), Time.deltaTime);
        }
        else if (backToStart)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, startingRotation, Time.deltaTime);
            if (Quaternion.Angle(transform.rotation, startingRotation) < 0.01f)
            {
                backToStart = false;
            }
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

    private void OnDestroy()
    {
        StopTracking();
    }
}