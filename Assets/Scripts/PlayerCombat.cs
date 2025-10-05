using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerEnergy))]
public class PlayerCombat : MonoBehaviour
{
    public Transform leftPupil;
    public Transform rightPupil;

    public LayerMask layersToHit;

    public GameObject beamPrefab;
    public float beamRange = 10f;
    public float damagePerSecond = 20f;

    public GameObject hitEffectPrefab;
    public GameObject clashEffectPrefab;

    private PlayerInput playerInput;
    private InputAction attackAction;
    private Camera mainCamera;
    private PlayerEnergy playerEnergy;
    private GameObject activeBeamLeft, activeBeamRight;
    private float damageTickTimer;

    public bool IsFiring()
    {
        return activeBeamLeft != null;
    }

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        attackAction = playerInput.actions["Attack"];
        mainCamera = Camera.main;
        playerEnergy = GetComponent<PlayerEnergy>();
    }

    private void Update()
    {
        float energyCostThisFrame = playerEnergy.energyDrainRate * Time.deltaTime;

        if (attackAction.IsPressed() && playerEnergy.HasEnoughEnergy(energyCostThisFrame))
        {
            playerEnergy.ConsumeEnergy(energyCostThisFrame);
            ActivateAndAimBeams();
        }
        else
        {
            DeactivateBeams();
        }
    }

    private void ActivateAndAimBeams()
    {
        if (activeBeamLeft == null) { activeBeamLeft = Instantiate(beamPrefab); }
        if (activeBeamRight == null) { activeBeamRight = Instantiate(beamPrefab); }

        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();
        Vector3 mouseScreenWithZ = new Vector3(mouseScreenPosition.x, mouseScreenPosition.y, 0 - mainCamera.transform.position.z);
        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(mouseScreenWithZ);

        ProcessBeam(activeBeamLeft, leftPupil, mouseWorldPosition);
        ProcessBeam(activeBeamRight, rightPupil, mouseWorldPosition);
    }

    private void ProcessBeam(GameObject beamInstance, Transform firePoint, Vector3 mouseTarget)
    {
        Vector2 aimDirection = (mouseTarget - firePoint.position).normalized;
        beamInstance.transform.position = firePoint.position;
        float worldAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        beamInstance.transform.rotation = Quaternion.Euler(0, 0, worldAngle);
        float distanceToMouse = Vector2.Distance(mouseTarget, firePoint.position);
        float effectiveDistance = Mathf.Min(distanceToMouse, beamRange);

        RaycastHit2D hitInfo = Physics2D.Raycast(firePoint.position, aimDirection, effectiveDistance, layersToHit);

        Vector2 endPoint;
        if (hitInfo.collider != null)
        {
            endPoint = hitInfo.point;

            if (hitInfo.collider.gameObject.layer == LayerMask.NameToLayer("EnemyBeam"))
            {
                if (clashEffectPrefab != null) { Instantiate(clashEffectPrefab, endPoint, Quaternion.identity); }
            }
            else if (hitInfo.collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                HandleHit(hitInfo.collider.gameObject, true);
                if (hitEffectPrefab != null) { Instantiate(hitEffectPrefab, endPoint, Quaternion.identity); }
            }
            else if (hitInfo.collider.gameObject.layer == LayerMask.NameToLayer("Ally"))
            {
                HandleHit(hitInfo.collider.gameObject, false);
            }
        }
        else
        {
            endPoint = (Vector2)firePoint.position + (aimDirection * effectiveDistance);
            damageTickTimer = 0;
        }

        float beamLength = Vector2.Distance(firePoint.position, endPoint);
        SpriteRenderer beamSprite = beamInstance.GetComponent<SpriteRenderer>();
        if (beamSprite != null) { beamSprite.size = new Vector2(beamLength, beamSprite.size.y); }
    }

    void HandleHit(GameObject target, bool isDamage)
    {
        damageTickTimer += Time.deltaTime;
        float damageInterval = 1f / 4f;

        if (damageTickTimer >= damageInterval)
        {
            float energyAmount = damagePerSecond * damageInterval;

            if (isDamage)
            {
                EnemyHealth enemy = target.GetComponent<EnemyHealth>();
                if (enemy != null) enemy.TakeDamage(Mathf.CeilToInt(energyAmount));
            }
            else
            {
                Ally ally = target.GetComponent<Ally>();
                if (ally != null) ally.Heal(energyAmount);
            }
            damageTickTimer = 0;
        }
    }

    private void DeactivateBeams()
    {
        if (activeBeamLeft != null) { Destroy(activeBeamLeft); activeBeamLeft = null; }
        if (activeBeamRight != null) { Destroy(activeBeamRight); activeBeamRight = null; }
    }
}