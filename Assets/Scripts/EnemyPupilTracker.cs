using UnityEngine;

public class EnemyPupilTracker : MonoBehaviour
{
    public Transform playerTarget;
    public float orbitRadius = 0.15f;

    private Transform rootTransform;
    private Vector3 localCenterPosition;

    void Awake()
    {
        rootTransform = transform.root;
    }

    void Start()
    {
        localCenterPosition = rootTransform.InverseTransformPoint(transform.position);
    }

    void Update()
    {
        if (playerTarget == null) return;

        Vector3 eyeCenterWorld = rootTransform.TransformPoint(localCenterPosition);
        Vector2 directionToTarget = (playerTarget.position - eyeCenterWorld).normalized;
        Vector2 targetPosition = (Vector2)eyeCenterWorld + (directionToTarget * orbitRadius);
        transform.position = Vector2.Lerp(transform.position, targetPosition, Time.deltaTime * 15f);
    }
}