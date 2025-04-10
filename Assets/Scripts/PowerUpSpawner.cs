using UnityEngine;

public class PowerUpSpawner : MonoBehaviour
{
    public GameObject[] powerUpPrefabs; // Array der verfügbaren Power-Up-Prefabs
    public float spawnInterval = 10f; // Zeitintervall zwischen Spawns
    public float minY = -3f; // Minimale Y-Position
    public float maxY = 3f; // Maximale Y-Position

    private void Start()
    {
        InvokeRepeating(nameof(SpawnPowerUp), spawnInterval, spawnInterval);
    }

    private void SpawnPowerUp()
    {
        int randomIndex = Random.Range(0, powerUpPrefabs.Length);
        float randomY = Random.Range(minY, maxY);

        Vector3 spawnPosition = new Vector3(15f, randomY, 0f); // Spawne rechts außerhalb des Bildschirms
        Instantiate(powerUpPrefabs[randomIndex], spawnPosition, Quaternion.identity);
    }
}
