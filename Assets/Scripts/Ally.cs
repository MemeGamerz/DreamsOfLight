using UnityEngine;

public class Ally : MonoBehaviour
{
    public float energyToHeal = 500f;
    public float currentEnergy { get; private set; } = 0f;
    private bool isHealed = false;

    public void Heal(float energyAmount)
    {
        if (isHealed) return;
        currentEnergy += energyAmount;
        if (currentEnergy >= energyToHeal)
        {
            isHealed = true;
            FinalSceneController finalScene = FindFirstObjectByType<FinalSceneController>();
            if (finalScene != null)
            {
                finalScene.OnLovedOneHealed();
            }
        }
    }
}