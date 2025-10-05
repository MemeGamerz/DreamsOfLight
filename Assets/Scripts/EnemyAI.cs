using UnityEngine;

[RequireComponent(typeof(EnemyHealth))]
public class EnemyAI : MonoBehaviour
{
    public Transform playerTarget;
    public float attackRange = 15f;
    public Transform leftPupil;
    public Transform rightPupil;
    public GameObject beamPrefab;
    public float beamEnergyCost = 50f;
    public LayerMask layersToHit;
    public GameObject clashEffectPrefab;
    public GameObject hitEffectPrefab;

    private Vector3 originalScale;
    private GameObject activeBeamLeft, activeBeamRight;
    private EnemyHealth enemyHealth;
    private EnemyPupilTracker[] pupilTrackers;

    void Awake()
    {
        enemyHealth = GetComponent<EnemyHealth>();
        pupilTrackers = GetComponentsInChildren<EnemyPupilTracker>();
        originalScale = transform.localScale;
    }

    void Update()
    {
        if (playerTarget == null) return;

        FlipTowardsTarget();

        float distanceToPlayer = Vector2.Distance(transform.position, playerTarget.position);

        if (enemyHealth.health > 0 && distanceToPlayer <= attackRange)
        {
            ActivateAndAimBeams();
        }
        else
        {
            DeactivateBeams();
        }
    }

    public void SetTarget(Transform newTarget)
    {
        playerTarget = newTarget;
        SetPupilTarget(playerTarget);
    }

    void FlipTowardsTarget()
    {
        if (playerTarget.position.x < transform.position.x)
        {
            transform.localScale = new Vector3(-Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
        }
        else
        {
            transform.localScale = new Vector3(Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
        }
    }

    void SetPupilTarget(Transform target)
    {
        foreach (var tracker in pupilTrackers)
        {
            tracker.playerTarget = target;
        }
    }

    void ActivateAndAimBeams()
    {
        if (activeBeamLeft == null) { activeBeamLeft = Instantiate(beamPrefab); }
        if (activeBeamRight == null) { activeBeamRight = Instantiate(beamPrefab); }
        ProcessBeam(activeBeamLeft, leftPupil, playerTarget.position);
        ProcessBeam(activeBeamRight, rightPupil, playerTarget.position);
    }

    void ProcessBeam(GameObject beamInstance, Transform firePoint, Vector3 targetPosition)
    {
        Vector2 aimDirection = (targetPosition - firePoint.position).normalized;
        beamInstance.transform.position = firePoint.position;
        float worldAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        beamInstance.transform.rotation = Quaternion.Euler(0, 0, worldAngle);
        RaycastHit2D hitInfo = Physics2D.Raycast(firePoint.position, aimDirection, attackRange, layersToHit);
        Vector2 endPoint;
        if (hitInfo.collider != null)
        {
            endPoint = hitInfo.point;
            if (hitInfo.collider.gameObject.layer == LayerMask.NameToLayer("PlayerBeam"))
            {
                if (clashEffectPrefab != null) { Instantiate(clashEffectPrefab, endPoint, Quaternion.identity); }
            }
            else if (hitInfo.collider.CompareTag("Player"))
            {
                PlayerEnergy playerEnergy = hitInfo.collider.GetComponent<PlayerEnergy>();
                if (playerEnergy != null)
                {
                    playerEnergy.TakeEnergy(beamEnergyCost * Time.deltaTime);
                }
                if (hitEffectPrefab != null) { Instantiate(hitEffectPrefab, endPoint, Quaternion.identity); }
            }
        }
        else
        {
            endPoint = (Vector2)firePoint.position + (aimDirection * attackRange);
        }
        float beamLength = Vector2.Distance(firePoint.position, endPoint);
        SpriteRenderer beamSprite = beamInstance.GetComponent<SpriteRenderer>();
        if (beamSprite != null) { beamSprite.size = new Vector2(beamLength, beamSprite.size.y); }
    }

    public void DeactivateBeams()
    {
        if (activeBeamLeft != null) { Destroy(activeBeamLeft); activeBeamLeft = null; }
        if (activeBeamRight != null) { Destroy(activeBeamRight); activeBeamRight = null; }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}