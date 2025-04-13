using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public enum PowerUpType
    {
        Shield,
        SpeedBoost
    }

    [SerializeField] public PowerUpType powerUpType; // Typ des PowerUps
    [SerializeField] public float duration = 5f; // Dauer des PowerUp-Effekts

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.AddPowerUpPoints();
            }

            CollisionDetection collisionDetection = collision.GetComponent<CollisionDetection>();
            if (collisionDetection != null)
            {
                if (powerUpType == PowerUpType.SpeedBoost)
                {
                    collisionDetection.ActivateSpeedBoost(duration);
                }
                else if (powerUpType == PowerUpType.Shield)
                {
                    collisionDetection.ActivateShield(duration);
                }
            }
            else
            {
                Debug.LogError("Keine CollisionDetection-Komponente am Player gefunden!");
            }

            Destroy(gameObject);
        }
    }
}
