using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerEnergy))]
public class PlayerCombat : MonoBehaviour
{
    public bool inCutscene = false;
    public Transform leftPupil, rightPupil;
    public LayerMask layersToHit;
    public GameObject beamPrefab;
    public float beamRange = 10f;
    public float damagePerSecond = 20f;
    public GameObject hitEffectPrefab, clashEffectPrefab;

    private PlayerInput playerInput;
    private InputAction attackAction;
    private Camera mainCamera;
    private PlayerEnergy playerEnergy;
    private GameObject activeBeamLeft, activeBeamRight;
    private float damageTickTimer;

    public bool IsFiring() { return activeBeamLeft != null; }

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        attackAction = playerInput.actions["Attack"];
        mainCamera = Camera.main;
        playerEnergy = GetComponent<PlayerEnergy>();
    }

    private void Update()
    {
        if (inCutscene) return;

        bool wantsToFire = attackAction.IsPressed();
        float energyCostThisFrame = playerEnergy.energyDrainRate * Time.deltaTime;

        if (wantsToFire && playerEnergy.HasEnoughEnergy(energyCostThisFrame))
        {
            playerEnergy.ConsumeEnergy(energyCostThisFrame);
            ActivateAndAimBeams(Mouse.current.position.ReadValue());
        }
        else
        {
            DeactivateBeams();
        }
    }

    public void ActivateAndAimBeams(Vector3 targetPosition, bool isScreenSpace = true)
    {
        if (activeBeamLeft == null) activeBeamLeft = Instantiate(beamPrefab);
        if (activeBeamRight == null) activeBeamRight = Instantiate(beamPrefab);

        Vector3 finalTargetPos = targetPosition;
        if (isScreenSpace)
        {
            Vector3 screenWithZ = new Vector3(targetPosition.x, targetPosition.y, 0 - mainCamera.transform.position.z);
            finalTargetPos = mainCamera.ScreenToWorldPoint(screenWithZ);
        }

        ProcessBeam(activeBeamLeft, leftPupil, finalTargetPos);
        ProcessBeam(activeBeamRight, rightPupil, finalTargetPos);
    }

    private void ProcessBeam(GameObject beamInstance, Transform firePoint, Vector3 mouseTarget)
    {
        Vector2 aimDirection = (mouseTarget - firePoint.position).normalized;
        beamInstance.transform.position = firePoint.position;
        float worldAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        beamInstance.transform.rotation = Quaternion.Euler(0, 0, worldAngle);
        float distanceToTarget = Vector2.Distance(mouseTarget, firePoint.position);
        float effectiveDistance = Mathf.Min(distanceToTarget, beamRange);
        RaycastHit2D hitInfo = Physics2D.Raycast(firePoint.position, aimDirection, effectiveDistance, layersToHit);
        Vector2 endPoint;
        if (hitInfo.collider != null)
        {
            endPoint = hitInfo.point;
            if (hitInfo.collider.gameObject.layer == LayerMask.NameToLayer("EnemyBeam"))
            {
                if (clashEffectPrefab != null) Instantiate(clashEffectPrefab, endPoint, Quaternion.identity);
            }
            else if (hitInfo.collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                DealContinuousDamage(hitInfo.collider.gameObject);
                if (hitEffectPrefab != null) Instantiate(hitEffectPrefab, endPoint, Quaternion.identity);
            }
        }
        else
        {
            endPoint = (Vector2)firePoint.position + (aimDirection * effectiveDistance);
            damageTickTimer = 0;
        }
        float beamLength = Vector2.Distance(firePoint.position, endPoint);
        SpriteRenderer beamSprite = beamInstance.GetComponent<SpriteRenderer>();
        if (beamSprite != null) beamSprite.size = new Vector2(beamLength, beamSprite.size.y);
    }

    void DealContinuousDamage(GameObject target)
    {
        damageTickTimer += Time.deltaTime;
        float damageInterval = 1f / 4f;
        if (damageTickTimer >= damageInterval)
        {
            float energyAmount = damagePerSecond * damageInterval;
            EnemyHealth enemy = target.GetComponent<EnemyHealth>();
            if (enemy != null) enemy.TakeDamage(Mathf.CeilToInt(energyAmount));
            damageTickTimer = 0;
        }
    }

    public void DeactivateBeams()
    {
        if (activeBeamLeft != null) { Destroy(activeBeamLeft); activeBeamLeft = null; }
        if (activeBeamRight != null) { Destroy(activeBeamRight); activeBeamRight = null; }
    }
}