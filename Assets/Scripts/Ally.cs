using UnityEngine;

public class Ally : MonoBehaviour
{
    public float energyToHeal = 500f;
    private float currentEnergy = 0f;
    private bool isHealed = false;

    public void Heal(float energyAmount)
    {
        if (isHealed) return;

        currentEnergy += energyAmount;

        if (currentEnergy >= energyToHeal)
        {
            isHealed = true;

            LevelManager levelManager = FindFirstObjectByType<LevelManager>();
            if (levelManager != null)
            {
                levelManager.OnAllyHealed();
            }
        }
    }
}