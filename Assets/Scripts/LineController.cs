using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineController : MonoBehaviour
{
    private LineRenderer lineRenderer;
    [SerializeField] private Transform[] points;
    
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

     void Update()
    {
        lineRenderer.positionCount = points.Length;
        for (int i = 0; i < points.Length; i++)
        {
            lineRenderer.SetPosition(i,points[i].position);
        }
    }
}
