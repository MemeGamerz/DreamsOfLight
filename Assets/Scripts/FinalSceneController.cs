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

    public float delayBeforeExplosion = 2f;
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

        Debug.Log("Player is sacrificing...");
        yield return new WaitForSeconds(delayBeforeExplosion);

        Instantiate(playerExplosionPrefab, player.transform.position, Quaternion.identity);
        Destroy(player);

        yield return new WaitForSeconds(delayAfterExplosion);
        lovedOneBody.SetActive(false);
        lovedOneAlive.SetActive(true);
        Debug.Log("She is alive!");

        DialogManager.instance.ShowDialog(finalDialog, ShowEndScreen);
    }

    void ShowEndScreen()
    {
        Debug.Log("THE END, THANKS FOR PLAYING!");
    }
}