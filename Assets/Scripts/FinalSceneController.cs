using System.Collections;
using UnityEngine;

public class FinalSceneController : MonoBehaviour
{
    public GameObject lovedOneBody;
    public GameObject lovedOneAlive;
    public GameObject playerExplosionPrefab;
    public GameObject endScreenPanel;
    public string playerDialog = "I can fix everyone but not you, I will try my best...";
    public string finalDialog = "You... You saved me. But at what cost?";
    public float delayAfterExplosion = 2f;

    private GameObject player;

    void Start()
    {
        lovedOneAlive.SetActive(false);
        if (endScreenPanel != null) endScreenPanel.SetActive(false);
    }

    public void StartFinalSequence()
    {
        StartCoroutine(FinalSequenceCoroutine());
    }

    private IEnumerator FinalSequenceCoroutine()
    {
        player = GameObject.FindWithTag("Player");
        if (player == null) yield break;

        PlayerEnergy playerEnergy = player.GetComponent<PlayerEnergy>();
        Ally lovedOne = lovedOneBody.GetComponent<Ally>();

        if (playerEnergy == null || lovedOne == null) yield break;

        player.GetComponent<PlayerController>().enabled = false;
        if (player.GetComponent<PlayerCombat>() != null) player.GetComponent<PlayerCombat>().enabled = false;

        DialogManager.instance.ShowDialog(playerDialog, null);
        // This line will now work correctly.
        yield return new WaitUntil(() => !DialogManager.instance.IsVisible);

        while (lovedOne.currentEnergy < lovedOne.energyToHeal)
        {
            if (playerEnergy.currentEnergy <= 0)
            {
                break;
            }
            float energyToTransfer = playerEnergy.maxEnergy * Time.deltaTime;
            playerEnergy.TakeEnergy(energyToTransfer);
            lovedOne.Heal(energyToTransfer);
            yield return null;
        }

        if (player != null)
        {
            Instantiate(playerExplosionPrefab, player.transform.position, Quaternion.identity);
            Destroy(player);
        }
    }

    public void OnLovedOneHealed()
    {
        StartCoroutine(HealingCompleteSequence());
    }

    private IEnumerator HealingCompleteSequence()
    {
        lovedOneBody.SetActive(false);
        lovedOneAlive.SetActive(true);
        yield return new WaitForSeconds(delayAfterExplosion);
        DialogManager.instance.ShowDialog(finalDialog, ShowEndScreen);
    }

    void ShowEndScreen()
    {
        if (endScreenPanel != null)
        {
            endScreenPanel.SetActive(true);
        }
    }
}