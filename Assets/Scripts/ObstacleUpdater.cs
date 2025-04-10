using UnityEngine;

public class ObstacleUpdater : MonoBehaviour
{
    private Rigidbody2D rb; // Referenz zum Rigidbody2D des Hindernisses
    private float currentSpeed; // Aktuelle Geschwindigkeit des Hindernisses

    void Start()
    {
        // Initialisiere die Rigidbody-Komponente
        rb = GetComponent<Rigidbody2D>();
    }

    public void SetSpeed(float speed)
    {
        // Setze die aktuelle Geschwindigkeit
        currentSpeed = speed;
    }

    void Update()
    {
        // Aktualisiere die Geschwindigkeit des Hindernisses
        if (rb != null)
        {
            rb.linearVelocity = Vector2.left * currentSpeed;
        }
    }
}
