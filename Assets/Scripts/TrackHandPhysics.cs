using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackHandPhysics : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;

    [SerializeField] private Transform targetHand;
    
    [SerializeField] private float positionSpeedMultiplier = 1.5f;
    
    [Range(0.0f, 1.0f)]
    [SerializeField] private float angularVelocityDamping = 1f;
    
    [SerializeField] private float angularVelocity = 1f;
    [SerializeField] private float maxAngularVelocity = 7f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.velocity = (targetHand.position - transform.position) * positionSpeedMultiplier / Time.fixedDeltaTime;
      
        rb.maxAngularVelocity = maxAngularVelocity;
        rb.angularVelocity *= (1f - angularVelocityDamping);
        Quaternion rotationDiff = targetHand.rotation * Quaternion.Inverse(transform.rotation);
        rotationDiff.ToAngleAxis(out float angle, out Vector3 axis);
        if (angle > 180f)
            angle -= 360f;
        
        if (Mathf.Abs(angle) > Mathf.Epsilon)
        {
            Vector3 rotationDifferance = angle * axis;
            Vector3 angleVelocity = (rotationDifferance * Mathf.Deg2Rad) / Time.fixedDeltaTime;
            rb.angularVelocity = angularVelocity * angleVelocity;
        }
    }
}
