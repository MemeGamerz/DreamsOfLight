using System.Collections;
using UnityEngine;

public class FinalSceneController : MonoBehaviour
{
    public GameObject player;
    public PlayerEnergy playerEnergy;
    public GameObject lovedOneBody;
    public GameObject lovedOneAlive;
    public GameObject playerExplosionPrefab;
    public string playerDialog = "I can fix everyone but not you, I will try my best...";
    public string finalDialog = "You... You saved me. But at what cost?";
    public float delayAfterExplosion = 2f;

    void Start()
    {
        lovedOneAlive.SetActive(false);
    }

    public void StartFinalSequence()
    {
        StartCoroutine(FinalSequenceCoroutine());
    }

    private IEnumerator FinalSequenceCoroutine()
    {
        player.GetComponent<PlayerController>().enabled = false;
        player.GetComponent<PlayerCombat>().enabled = false;

        DialogManager.instance.ShowDialog(playerDialog, null);
        yield return new WaitUntil(() => !DialogManager.instance.dialogPanel.activeInHierarchy);

        Ally lovedOne = lovedOneBody.GetComponent<Ally>();
        PlayerEnergy energySource = player.GetComponent<PlayerEnergy>();

        if (lovedOne != null && energySource != null)
        {
            while (energySource.currentEnergy > 0)
            {
                float energyToTransfer = energySource.maxEnergy * Time.deltaTime;
                energySource.TakeEnergy(energyToTransfer);
                lovedOne.Heal(energyToTransfer);
                yield return null;
            }
        }

        Instantiate(playerExplosionPrefab, player.transform.position, Quaternion.identity);
        Destroy(player);
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
        Debug.Log("THE END, THANKS FOR PLAYING!");
    }
}