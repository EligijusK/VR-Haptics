using UnityEngine;

public class CornerController : MonoBehaviour
{
    [SerializeField] private Transform targetTransform;
    [SerializeField] private Collider cornerTrigger;
    [SerializeField] public bool isForClamps = true;
    public bool isTaken = false;

    public Transform GetTargetTransform()
    {
        return targetTransform;
    }

    public Collider GetTriggerCollider()
    {
        return cornerTrigger;
    }
}