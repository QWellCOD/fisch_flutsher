using UnityEngine;

public class ObstacleMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // Geschwindigkeit der horizontalen Bewegung

    void Update()
    {
        // Bewege das Hindernis nach links
        transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);

        // Zerstöre das Hindernis, wenn es außerhalb des Bildschirms ist
        if (transform.position.x < -15f) // Passe den Wert je nach Bildschirmgröße an
        {
            Destroy(gameObject);
        }
    }
}
