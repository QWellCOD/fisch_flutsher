using UnityEngine;

public class MineSpawner : MonoBehaviour
{
    [Header("Obstacle Settings")]
    public GameObject[] obstaclePrefabs;

    [Header("Spawn Settings")]
    public float initialObstacleSpawnTime = 3f; 
    public float minimumObstacleSpawnTime = 0.3f;
    public float timeToReachMinimum = 120f;

    [Header("Spawn Position")]
    public float minSpawnHeight = -1f;
    public float maxSpawnHeight = 1.5f;

    [Header("Debug")]
    public bool showDebugInfo = false;

    private float currentObstacleSpawnTime;
    private float timeUntilObstacleSpawn = 0f;
    private float gameTimer = 0f;

    void Start()
    {
        currentObstacleSpawnTime = initialObstacleSpawnTime;

        if (showDebugInfo)
        {
            Debug.Log($"MineSpawner initialisiert - Startintervall: {initialObstacleSpawnTime}s, " +
                      $"Minimales Intervall: {minimumObstacleSpawnTime}s, " +
                      $"Zeit bis Minimum: {timeToReachMinimum}s");
        }
    }

    void Update()
    {
        gameTimer += Time.deltaTime;

        AdjustSpawnRateByTime();

        SpawnLoop();
    }

    private void AdjustSpawnRateByTime()
    {
        float progressFactor = Mathf.Clamp01(gameTimer / timeToReachMinimum);

        currentObstacleSpawnTime = Mathf.Lerp(initialObstacleSpawnTime,
                                             minimumObstacleSpawnTime,
                                             progressFactor);

        if (showDebugInfo && Mathf.FloorToInt(gameTimer) % 5 == 0 && Time.frameCount % 60 == 0)
        {
            Debug.Log($"Spielzeit: {gameTimer:F1}s, " +
                      $"Fortschritt: {progressFactor:P0}, " +
                      $"Aktuelles Spawn-Intervall: {currentObstacleSpawnTime:F2}s");
        }
    }

    private void SpawnLoop()
    {
        timeUntilObstacleSpawn += Time.deltaTime;

        if (timeUntilObstacleSpawn >= currentObstacleSpawnTime)
        {
            Spawn();
            timeUntilObstacleSpawn = 0f;
        }
    }

    private void Spawn()
    {
        GameObject obstacleToSpawn = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)];

        float randomY = Random.Range(minSpawnHeight, maxSpawnHeight);

        Vector3 spawnPosition = new Vector3(transform.position.x, randomY, transform.position.z);

        GameObject spawnedObstacle = Instantiate(obstacleToSpawn, spawnPosition, Quaternion.identity);

        if (showDebugInfo)
        {
            Debug.Log($"Obstacle spawned at position: {spawnPosition}, " +
                      $"Current interval: {currentObstacleSpawnTime:F2}s");
        }
    }

    public void AccelerateSpawnRate(float accelerationFactor = 0.8f)
    {
        currentObstacleSpawnTime *= accelerationFactor;

        currentObstacleSpawnTime = Mathf.Max(currentObstacleSpawnTime, minimumObstacleSpawnTime);

        if (showDebugInfo)
        {
            Debug.Log($"Spawn-Rate beschleunigt. Neues Intervall: {currentObstacleSpawnTime:F2}s");
        }
    }

    public void ResetSpawnRate()
    {
        currentObstacleSpawnTime = initialObstacleSpawnTime;
        gameTimer = 0f;

        if (showDebugInfo)
        {
            Debug.Log("Spawn-Rate zurückgesetzt.");
        }
    }
}
