using UnityEngine;

public class MineSpawner : MonoBehaviour
{
    [Header("Obstacle Settings")]
    public GameObject[] obstaclePrefabs;

    [Header("Spawn Settings")]
    public float initialObstacleSpawnTime = 3f; // Reduziert von 5f auf 3f für häufigere Spawns zu Beginn
    public float minimumObstacleSpawnTime = 0.3f; // Reduziert von 0.5f auf 0.3f für höhere maximale Spawnrate
    public float timeToReachMinimum = 120f; // Zeit in Sekunden, bis die minimale Spawn-Zeit erreicht ist (2 Minuten)

    [Header("Spawn Position")]
    public float minSpawnHeight = -1f; // Minimale Y-Position
    public float maxSpawnHeight = 1f;  // Maximale Y-Position

    [Header("Debug")]
    public bool showDebugInfo = false; // Option zum Aktivieren von Debug-Ausgaben

    private float currentObstacleSpawnTime;
    private float timeUntilObstacleSpawn = 0f;
    private float gameTimer = 0f;

    void Start()
    {
        // Startzeit festlegen
        currentObstacleSpawnTime = initialObstacleSpawnTime;

        // Optionale Debug-Ausgabe
        if (showDebugInfo)
        {
            Debug.Log($"MineSpawner initialisiert - Startintervall: {initialObstacleSpawnTime}s, " +
                      $"Minimales Intervall: {minimumObstacleSpawnTime}s, " +
                      $"Zeit bis Minimum: {timeToReachMinimum}s");
        }
    }

    void Update()
    {
        // Spielzeit aktualisieren
        gameTimer += Time.deltaTime;

        // Spawn-Rate basierend auf der Spielzeit anpassen
        AdjustSpawnRateByTime();

        // Spawning-Loop
        SpawnLoop();
    }

    private void AdjustSpawnRateByTime()
    {
        // Berechne den Fortschritt (0 bis 1) basierend auf der Spielzeit
        float progressFactor = Mathf.Clamp01(gameTimer / timeToReachMinimum);

        // Lineare Interpolation zwischen Anfangs- und Minimalwert
        currentObstacleSpawnTime = Mathf.Lerp(initialObstacleSpawnTime,
                                             minimumObstacleSpawnTime,
                                             progressFactor);

        // Optionale Debug-Ausgabe alle 5 Sekunden
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
            timeUntilObstacleSpawn = 0f; // Timer zurücksetzen
        }
    }

    private void Spawn()
    {
        // Zufälliges Obstacle aus dem Array auswählen
        GameObject obstacleToSpawn = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)];

        // Zufällige Y-Position innerhalb des definierten Bereichs
        float randomY = Random.Range(minSpawnHeight, maxSpawnHeight);

        // Spawn-Position definieren
        Vector3 spawnPosition = new Vector3(transform.position.x, randomY, transform.position.z);

        // Obstacle instanziieren
        GameObject spawnedObstacle = Instantiate(obstacleToSpawn, spawnPosition, Quaternion.identity);

        // Optionale Debug-Ausgabe
        if (showDebugInfo)
        {
            Debug.Log($"Hindernis gespawnt bei Position: {spawnPosition}, " +
                      $"Aktuelles Intervall: {currentObstacleSpawnTime:F2}s");
        }
    }

    // Diese Methode kann von außen aufgerufen werden, um die Spawnrate manuell zu beschleunigen
    public void AccelerateSpawnRate(float accelerationFactor = 0.8f)
    {
        // Reduziert die aktuelle Spawn-Zeit um den angegebenen Faktor
        currentObstacleSpawnTime *= accelerationFactor;

        // Stellt sicher, dass die Spawn-Zeit nicht unter das Minimum fällt
        currentObstacleSpawnTime = Mathf.Max(currentObstacleSpawnTime, minimumObstacleSpawnTime);

        if (showDebugInfo)
        {
            Debug.Log($"Spawn-Rate beschleunigt. Neues Intervall: {currentObstacleSpawnTime:F2}s");
        }
    }

    // Diese Methode kann zum Reset der Spawn-Rate verwendet werden
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
