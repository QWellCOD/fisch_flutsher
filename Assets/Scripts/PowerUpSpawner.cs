using UnityEngine;

public class PowerUpSpawner : MonoBehaviour
{
    public GameObject[] powerUpPrefabs;
    public float spawnInterval = 10f;
    public float minY = -3f;
    public float maxY = 3f;

    private void Start()
    {
        InvokeRepeating(nameof(SpawnPowerUp), spawnInterval, spawnInterval);
    }


    private void SpawnPowerUp()
    {
        int randomIndex = Random.Range(0, powerUpPrefabs.Length);
        float randomY = Random.Range(minY, maxY);

        Vector3 spawnPosition = new Vector3(15f, randomY, 0f);
        Instantiate(powerUpPrefabs[randomIndex], spawnPosition, Quaternion.identity);
    }
}
