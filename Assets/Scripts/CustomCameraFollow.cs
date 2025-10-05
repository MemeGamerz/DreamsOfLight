using UnityEngine;

public class CustomCameraFollow : MonoBehaviour
{
    public Transform playerTarget;
    public float followSpeed = 5f;
    public float deadZoneX = 2f;

    void LateUpdate()
    {
        if (playerTarget == null)
        {
            return;
        }

        float distanceX = playerTarget.position.x - transform.position.x;

        if (Mathf.Abs(distanceX) > deadZoneX)
        {
            Vector3 targetPosition = new Vector3(playerTarget.position.x, transform.position.y, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
        }
    }
}