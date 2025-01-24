using UnityEngine;

public class PlungerColliderEvents : MonoBehaviour
{
    public delegate void PlungerAction();
    public event PlungerAction OnExtend;
    public event PlungerAction OnPush;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Plunger"))
        {
            // Determine which trigger was hit based on its position or name
            if (gameObject.name == "ExtendTrigger")
            {
                OnExtend?.Invoke();
            }
            else if (gameObject.name == "PushTrigger")
            {
                OnPush?.Invoke();
            }
        }
    }
}