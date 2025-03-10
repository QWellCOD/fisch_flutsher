using UnityEngine;

public class ObstacleMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // Geschwindigkeit der horizontalen Bewegung

    void Update()
    {
        // Bewege das Hindernis nach links
        transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);

        // Zerst�re das Hindernis, wenn es au�erhalb des Bildschirms ist
        if (transform.position.x < -15f) // Passe den Wert je nach Bildschirmgr��e an
        {
            Destroy(gameObject);
        }
    }
}
