using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // Geschwindigkeit der Bewegung
    public float tiltAngle = 30f; // Maximaler Neigungswinkel in Grad
    public float tiltSpeed = 5f; // Geschwindigkeit der Neigung

    private float currentRotation = 0f; // Aktuelle Rotation des Fischs

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

        // Anwenden der Bewegung im WELT-Koordinatensystem, unabhängig von der Rotation
        transform.Translate(Vector2.up * verticalInput * moveSpeed * Time.deltaTime, Space.World);

        // Kippwinkel berechnen
        float targetRotation = 0f;

        if (verticalInput != 0)
        {
            // Wenn Bewegung stattfindet, in Richtung der Bewegung kippen
            targetRotation = verticalInput * tiltAngle;
        }

        // Sanfter Übergang zur Zielrotation
        currentRotation = Mathf.Lerp(currentRotation, targetRotation, tiltSpeed * Time.deltaTime);

        // rotation
        transform.rotation = Quaternion.Euler(0, 0, currentRotation);
    }
}
