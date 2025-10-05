using UnityEngine;
using UnityEngine.InputSystem;

public class PupilTracker : MonoBehaviour
{
    public float orbitRadius = 0.15f;

    private Camera mainCamera;
    private Transform rootTransform;
    private Vector3 localCenterPosition;

    void Awake()
    {
        mainCamera = Camera.main;
        rootTransform = transform.root;
    }

    void Start()
    {
        localCenterPosition = rootTransform.InverseTransformPoint(transform.position);
    }

    void Update()
    {
        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector3 eyeCenterWorld = rootTransform.TransformPoint(localCenterPosition);
        Vector2 directionToMouse = (mouseWorldPosition - eyeCenterWorld).normalized;
        Vector2 targetPosition = (Vector2)eyeCenterWorld + (directionToMouse * orbitRadius);
        transform.position = Vector2.Lerp(transform.position, targetPosition, Time.deltaTime * 15f);
    }
}