using UnityEngine;

public class CornerController : MonoBehaviour
{
    [SerializeField] private Transform targetTransform; // The exact transform for the clamp to move to
    [SerializeField] private Collider cornerTrigger; // The child collider that detects proximity

    public Transform GetTargetTransform()
    {
        return targetTransform;
    }

    public Collider GetTriggerCollider()
    {
        return cornerTrigger;
    }
}