using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Cinemachine;

public class LevelManager : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public GameObject enemyPrefab;
        public GameObject deadAllyPrefab;
    }

    public Wave[] waves;
    public Transform enemySpawnPoint;
    public Transform allySpawnPoint;
    public string nextLevelName;

    public string endLevelDialog;

    public CinemachineCamera virtualCamera;

    private int currentWaveIndex = 0;
    private GameObject currentEnemy;
    private GameObject currentAllyInstance;

    void Start()
    {
        if (virtualCamera == null)
        {
            virtualCamera = FindFirstObjectByType<CinemachineCamera>();
        }

        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
        {
            Transform playerInstance = playerObject.transform;
            if (virtualCamera != null)
            {
                virtualCamera.Follow = playerInstance;
            }
        }

        StartNextWave();
    }

    void StartNextWave()
    {
        if (currentWaveIndex < waves.Length)
        {
            if (waves[currentWaveIndex].deadAllyPrefab != null)
            {
                currentAllyInstance = Instantiate(waves[currentWaveIndex].deadAllyPrefab, allySpawnPoint.position, Quaternion.identity);
            }

            if (waves[currentWaveIndex].enemyPrefab != null)
            {
                currentEnemy = Instantiate(waves[currentWaveIndex].enemyPrefab, enemySpawnPoint.position, Quaternion.identity);
            }

            Debug.Log("Wave " + (currentWaveIndex + 1) + ": Enemy and fallen ally have appeared!");
        }
        else
        {
            LevelComplete();
        }
    }

    public void OnAllyHealed()
    {
        Debug.Log("Ally Healed! Moving to next wave.");
        currentWaveIndex++;

        if (currentAllyInstance != null) { Destroy(currentAllyInstance); }
        if (currentEnemy != null) { Destroy(currentEnemy); }

        StartNextWave();
    }

    void LevelComplete()
    {
        Debug.Log("Level Complete!");
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