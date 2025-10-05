using System.Collections;
using UnityEngine;

public class FinalSceneController : MonoBehaviour
{
    public GameObject lovedOneBody;
    public GameObject lovedOneAlive;
    public GameObject playerExplosionPrefab;
    public GameObject endScreenPanel;
    public string playerDialog = "I can fix everyone but not you, I will try my best...";
    public string finalDialog = "You... You saved me. I will remember your motto 'Sacrifice Must be made'";
    public float delayAfterExplosion = 2f;
    public float healingDuration = 3f;

    private bool sequenceStarted = false;
    private DialogManager dialogManager;

    void Start()
    {
        dialogManager = FindFirstObjectByType<DialogManager>();
        lovedOneAlive.SetActive(false);
        if (endScreenPanel != null) endScreenPanel.SetActive(false);
        if (dialogManager != null && dialogManager.dialogPanel != null)
        {
            dialogManager.dialogPanel.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !sequenceStarted)
        {
            sequenceStarted = true;
            StartCoroutine(FinalSequenceCoroutine(other.gameObject));
        }
    }

    private IEnumerator FinalSequenceCoroutine(GameObject player)
    {
        if (player == null) yield break;
        PlayerEnergy playerEnergy = player.GetComponent<PlayerEnergy>();
        PlayerCombat playerCombat = player.GetComponent<PlayerCombat>();
        if (playerEnergy == null || playerCombat == null) yield break;

        player.GetComponent<PlayerController>().enabled = false;
        playerCombat.inCutscene = true;

        if (dialogManager != null)
        {
            dialogManager.ShowDialog(playerDialog, null);
            yield return new WaitUntil(() => dialogManager.dialogPanel == null || !dialogManager.dialogPanel.activeInHierarchy);
        }

        SpriteRenderer lovedOneRenderer = lovedOneBody.GetComponent<SpriteRenderer>();
        Material lovedOneMaterial = lovedOneRenderer.material;

        float timer = 0f;
        while (timer < healingDuration)
        {
            timer += Time.deltaTime;
            float energyToTransfer = (playerEnergy.maxEnergy / healingDuration) * Time.deltaTime;
            playerEnergy.TakeEnergy(energyToTransfer);
            playerCombat.ActivateAndAimBeams(lovedOneBody.transform.position, false);

            if (lovedOneMaterial.HasProperty("_FillAmount"))
            {
                // This line is now corrected to drain from 1 to 0.
                float fillProgress = 1.0f - (timer / healingDuration);
                lovedOneMaterial.SetFloat("_FillAmount", fillProgress);
            }

            yield return null;
        }

        playerCombat.DeactivateBeams();

        if (player != null)
        {
            Instantiate(playerExplosionPrefab, player.transform.position, Quaternion.identity);
            Destroy(player);
        }

        lovedOneBody.SetActive(false);
        lovedOneAlive.SetActive(true);
        yield return new WaitForSeconds(delayAfterExplosion);

        if (dialogManager != null)
        {
            dialogManager.ShowDialog(finalDialog, ShowEndScreen);
        }
    }

    void ShowEndScreen()
    {
        if (endScreenPanel != null)
        {
            endScreenPanel.SetActive(true);
        }
    }
}