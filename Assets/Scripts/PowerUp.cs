using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public enum PowerUpType
    {
        Shield,
        SpeedBoost
    }

    [SerializeField] public PowerUpType powerUpType;
    [SerializeField] public float duration = 5f;

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
                Debug.LogError("No CollisionDetection component found on the Player!");
            }

            Destroy(gameObject);
        }
    }
}
