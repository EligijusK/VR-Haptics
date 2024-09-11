using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class OpenDoors : MonoBehaviour
{
    [SerializeField] Vector3 addForce;
    [SerializeField]
    UnityEvent openDoorsEvent;
    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void AddForce()
    {
        rb.constraints = RigidbodyConstraints.None;
        rb.AddForce(addForce, ForceMode.Impulse);
        openDoorsEvent.Invoke();
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(OpenDoors))]
public class OpenDoorsEditor : Editor
{
    
    
    
    public override void OnInspectorGUI() {
        OpenDoors doors = (OpenDoors)target;
        
        base.OnInspectorGUI();
        
        if (GUILayout.Button("Test"))
        {
            doors.AddForce();
        }
    }
}

#endif
