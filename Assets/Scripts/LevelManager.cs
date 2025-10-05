using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Cinemachine;

public class LevelManager : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public GameObject enemyPrefab;
        public GameObject livingAllyPrefab;
    }

    public GameObject playerPrefab;
    public Transform playerSpawnPoint;
    public Wave[] waves;
    public Transform enemySpawnPoint;
    public string nextLevelName;
    public string endLevelDialog;
    public CinemachineCamera virtualCamera;

    private int currentWaveIndex = 0;
    private GameObject currentEnemy;

    void Start()
    {
        GameObject playerInstance = Instantiate(playerPrefab, playerSpawnPoint.position, Quaternion.identity);

        if (virtualCamera == null)
        {
            virtualCamera = FindFirstObjectByType<CinemachineCamera>();
        }
        if (virtualCamera != null && playerInstance != null)
        {
            virtualCamera.Follow = playerInstance.transform;
        }

        StartNextWave();
    }

    void StartNextWave()
    {
        if (currentWaveIndex < waves.Length)
        {
            currentEnemy = Instantiate(waves[currentWaveIndex].enemyPrefab, enemySpawnPoint.position, Quaternion.identity);
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