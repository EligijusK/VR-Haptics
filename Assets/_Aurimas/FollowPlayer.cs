using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform playerCamera; 
    public Vector3 offset = new Vector3(0, 0, 100f); 

    void Update()
    {
        transform.position = playerCamera.position + playerCamera.forward * offset.z + playerCamera.up * offset.y + playerCamera.right * offset.x;
        
        transform.rotation = Quaternion.LookRotation(transform.position - playerCamera.position);
    }
}