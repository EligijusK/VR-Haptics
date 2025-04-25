using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform playerCamera; 
    public Vector3 offset = new Vector3(0, 0, 100f); 

    void LateUpdate()
    {
        Vector3 horizontalForward = playerCamera.forward;
        horizontalForward.y = 0f;
        horizontalForward.Normalize();
        
        Vector3 targetPos = playerCamera.position
                            + horizontalForward * offset.z
                            + playerCamera.right    * offset.x
                            + Vector3.up            * offset.y;

        transform.position = targetPos;
        
        Vector3 lookDir = playerCamera.position - transform.position;
        lookDir.y = 0f;
        if (lookDir.sqrMagnitude > 0.001f)
        {
            transform.rotation = Quaternion.LookRotation(-lookDir, Vector3.up);
        }
    }
}