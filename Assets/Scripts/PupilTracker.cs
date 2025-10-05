using UnityEngine;
using UnityEngine.InputSystem;

public class PupilTracker : MonoBehaviour
{
    public float orbitRadius = 0.15f;

    private Camera mainCamera;
    private Transform rootTransform;
    private Vector3 localRestingPosition;

    void Awake()
    {
        mainCamera = Camera.main;
        rootTransform = transform.root;
    }

    void Start()
    {
        localRestingPosition = rootTransform.InverseTransformPoint(transform.position);
    }



    void Update()
    {
        if (mainCamera == null) mainCamera = Camera.main;
        if (mainCamera == null) return;

        Vector3 worldRestingPosition = rootTransform.TransformPoint(localRestingPosition);
        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2 directionToMouse = (mouseWorldPosition - worldRestingPosition).normalized;
        Vector2 targetPosition = (Vector2)worldRestingPosition + (directionToMouse * orbitRadius);

        transform.position = Vector2.Lerp(transform.position, targetPosition, Time.deltaTime * 15f);
    }
}