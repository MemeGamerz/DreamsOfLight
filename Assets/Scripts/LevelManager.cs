using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public GameObject playerPrefab;
    public Transform playerSpawnPoint;
    public Wave[] waves;
    public Transform enemySpawnPoint;
    public string nextLevelName;
    public string endLevelDialog;
    private CustomCameraFollow cameraFollowScript;

    [System.Serializable]
    public class Wave
    {
        public GameObject enemyPrefab;
        public GameObject livingAllyPrefab;
    }

    private int currentWaveIndex = 0;
    private GameObject currentEnemy;
    private GameObject playerInstance;

    void Start()
    {
        playerInstance = Instantiate(playerPrefab, playerSpawnPoint.position, Quaternion.identity);

        cameraFollowScript = FindFirstObjectByType<CustomCameraFollow>();
        if (cameraFollowScript != null)
        {
            cameraFollowScript.playerTarget = playerInstance.transform;
        }
        StartNextWave();
    }

    void StartNextWave()
    {
        if (currentWaveIndex < waves.Length)
        {
            currentEnemy = Instantiate(waves[currentWaveIndex].enemyPrefab, enemySpawnPoint.position, Quaternion.identity);
            EnemyAI ai = currentEnemy.GetComponent<EnemyAI>();

            if (ai != null && playerInstance != null)
            {
                ai.SetTarget(playerInstance.transform);
            }
        }
        else
        {
            LevelComplete();
        }
    }

    public void OnEnemyKilled(Vector3 deathPosition)
    {
        GameObject allyInstance = Instantiate(waves[currentWaveIndex].livingAllyPrefab, deathPosition, Quaternion.identity);
        DialogManager.instance.ShowDialog("Thank you for freeing me!", () =>
        {
            currentWaveIndex++;
            Destroy(allyInstance);
            StartNextWave();
        });
    }

    void LevelComplete()
    {
        DialogManager.instance.ShowDialog(endLevelDialog, LoadNextLevel);
    }

    void LoadNextLevel()
    {
        if (!string.IsNullOrEmpty(nextLevelName))
        {
            SceneManager.LoadScene(nextLevelName);
        }
    }
}