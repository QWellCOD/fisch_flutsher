using UnityEngine;
using UnityEngine.SceneManagement;

public class CollisionDetection : MonoBehaviour
{
    public GameObject gameOverScreen; // Verweis auf das Game Over UI

    void OnTriggerEnter2D(Collider2D other)
    {
        // Prüfen, ob das kollidierte Objekt den Tag "Obstacle" hat
        if (other.CompareTag("Obstacle"))
        {
            Debug.Log("Collision with Obstacle detected!"); // Debug-Ausgabe zur Überprüfung
            GameOver(); // Spiel beenden
        }

        //// Platz für weitere Tags (z. B. Gegner)
        //if (other.CompareTag("Enemy"))
        //{
        //    Debug.Log("Collision with Enemy detected!");
        //    // Weitere Logik für Gegner hier einfügen
        //}

        //if (other.CompareTag("PowerUp"))
        //{
        //    Debug.Log("Collision with PowerUp detected!");
        //    // Logik für PowerUps hier einfügen
        //}

    }

    void GameOver()
    {
        // Aktiviert das Game Over UI
        if (gameOverScreen != null)
        {
            Debug.Log("Activating Game Over Screen...");
            gameOverScreen.SetActive(true);
        }
        else
        {
            Debug.LogError("Game Over Panel is not assigned!");
        }

        // Spiel pausieren
        Time.timeScale = 0;
    }

    public void RestartGame()
    {
        // Spiel neu starten
        Debug.Log("Restart button clicked!");
        Time.timeScale = 1; // Zeit wieder aktivieren
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Aktuelle Szene neu laden
    }

    public void HomeScene()
    {
        // Zurück zum Hauptmenü
        Time.timeScale = 1; // Zeit wieder aktivieren
        SceneManager.LoadScene("HomeScene"); // Passe den Szenennamen an
    }
}
