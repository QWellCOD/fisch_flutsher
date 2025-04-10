using UnityEngine;

public class MineSpawner : MonoBehaviour
{
    [Header("Obstacle Settings")]
    public GameObject[] obstaclePrefabs; // Liste der Hindernis-Prefabs
    public float obstacleSpawnTime = 5f; // Zeit zwischen Spawns
    //public float obstacleSpeed = 5f;     // Geschwindigkeit der Hindernisse

    private float timeUntilObstacleSpawn = 0f;

    void Update()
    {
        SpawnLoop();
    }

    private void SpawnLoop()
    {
        timeUntilObstacleSpawn += Time.deltaTime;

        if (timeUntilObstacleSpawn >= obstacleSpawnTime)
        {
            Spawn();
            timeUntilObstacleSpawn = 0f; // Timer zur�cksetzen
        }
    }

    private void Spawn()
    {
        GameObject obstacleToSpawn = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)];

        // Zufällige Y-Position innerhalb eines Bereichs
        float randomY = Random.Range(-1f, 1f); // Passe den Wertebereich an deine Szene an

        Vector3 spawnPosition = new Vector3(transform.position.x, randomY, transform.position.z);
        GameObject spawnedObstacle = Instantiate(obstacleToSpawn, spawnPosition, Quaternion.identity);

        Debug.Log("Spawned Obstacle Position: " + spawnPosition); // Debug-Ausgabe

        Rigidbody2D obstacleRB = spawnedObstacle.GetComponent<Rigidbody2D>();
        //if (obstacleRB != null)
        //{
        //    obstacleRB.linearVelocity = Vector2.left * obstacleSpeed; // Bewegung nach links
        //}
    }
}