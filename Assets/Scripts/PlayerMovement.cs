using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // Geschwindigkeit der Bewegung

    void Update()
    {
        // Bewegung nach oben und unten basierend auf Tasteneingaben
        float verticalInput = 0;

        // A und Pfeil Links bewegen den Fisch nach oben
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            verticalInput = 1;
        }
        // D und Pfeil Rechts bewegen den Fisch nach unten
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            verticalInput = -1;
        }

        // Anwenden der Bewegung
        transform.Translate(Vector2.up * verticalInput * moveSpeed * Time.deltaTime);
    }
}
