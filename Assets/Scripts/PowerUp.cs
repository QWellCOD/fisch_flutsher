using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public enum PowerUpType { Shield, SpeedBoost } // Verschiedene Arten von Power-Ups
    public PowerUpType powerUpType; // Typ des aktuellen Power-Ups
    public float duration = 5f; // Dauer des Effekts

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Prüfen, ob der Spieler das Power-Up berührt
        {
            ApplyEffect(other.gameObject); // Effekt anwenden
            Destroy(gameObject); // Power-Up entfernen
        }
    }

    private void ApplyEffect(GameObject player)
    {
        CollisionDetection playerController = player.GetComponent<CollisionDetection>();

        if (playerController != null)
        {
            switch (powerUpType)
            {
                case PowerUpType.Shield:
                    playerController.ActivateShield(duration);
                    break;
                case PowerUpType.SpeedBoost:
                    playerController.ActivateSpeedBoost(duration);
                    break;
            }
        }
    }
}
