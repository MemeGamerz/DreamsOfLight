using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerEnergy : MonoBehaviour
{
    public float maxEnergy = 1000f;
    public float energyDrainRate = 20f;
    public float energyRegenRate = 10f;
    public int startingBoosters = 3;
    public SpriteRenderer bodySpriteRenderer;

    public float currentEnergy { get; private set; }
    public int currentBoosters { get; private set; }

    private Material energyMaterial;
    private PlayerInput playerInput;
    private InputAction healAction;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        healAction = playerInput.actions["Heal"];
    }

    void Start()
    {
        currentEnergy = maxEnergy;
        currentBoosters = startingBoosters;
        if (bodySpriteRenderer != null)
        {
            energyMaterial = bodySpriteRenderer.material;
        }
    }

    private void OnEnable()
    {
        healAction.performed += OnHealPerformed;
    }

    private void OnDisable()
    {
        healAction.performed -= OnHealPerformed;
    }

    void Update()
    {
        PlayerCombat combat = GetComponent<PlayerCombat>();
        if (combat == null || !combat.IsFiring())
        {
            RegenerateEnergy();
        }
        UpdateEnergyVisuals();
    }

    private void OnHealPerformed(InputAction.CallbackContext context)
    {
        UseEnergyBooster();
    }

    public void UseEnergyBooster()
    {
        if (currentBoosters > 0 && currentEnergy < maxEnergy)
        {
            currentEnergy = maxEnergy;
            currentBoosters--;
        }
    }

    private void RegenerateEnergy()
    {
        if (currentEnergy < maxEnergy)
        {
            currentEnergy += energyRegenRate * Time.deltaTime;
            currentEnergy = Mathf.Min(currentEnergy, maxEnergy);
        }
    }

    private void UpdateEnergyVisuals()
    {
        if (energyMaterial != null)
        {
            float fillAmount = currentEnergy / maxEnergy;
            energyMaterial.SetFloat("_FillAmount", fillAmount);
        }
    }

    public bool HasEnoughEnergy(float amountNeeded)
    {
        return currentEnergy >= amountNeeded;
    }

    public void ConsumeEnergy(float amount)
    {
        currentEnergy -= amount;
        if (currentEnergy < 0)
        {
            currentEnergy = 0;
        }
    }

    public void TakeEnergy(float amount)
    {
        currentEnergy -= amount;
        if (currentEnergy < 0)
        {
            currentEnergy = 0;
        }
    }
}