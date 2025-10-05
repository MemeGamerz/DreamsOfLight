using UnityEngine;

public class TriggerZone : MonoBehaviour
{
    private bool hasBeenTriggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!hasBeenTriggered && other.CompareTag("Player"))
        {
            hasBeenTriggered = true;
            FinalSceneController sceneController = FindFirstObjectByType<FinalSceneController>();
            if (sceneController != null)
            {
                sceneController.StartFinalSequence();
            }

            gameObject.SetActive(false);
        }
    }
}