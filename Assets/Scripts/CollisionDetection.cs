using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine.Audio;

public class CollisionDetection : MonoBehaviour
{
    [Header("UI References")]
    //public GameObject gameOverScreen;
    public GameObject shieldVisual;

    [Header("Settings")]
    [SerializeField] private float speedBoostMultiplier = 2f;

    private bool isShieldActive = false;
    private float shieldDurationRemaining = 0f;
    private PlayerMovement movement;
    private float baseMoveSpeed;
    public AudioMixerSnapshot snapshot;

    private bool isSpeedBoostActive = false;
    private float speedBoostDurationRemaining = 0f;

    public ParticleSystem speedBoostParticles;

    void Start()
    {
        movement = GetComponent<PlayerMovement>();
        baseMoveSpeed = movement.moveSpeed;

        if (shieldVisual) shieldVisual.SetActive(false);
    }

    void Update()
    {
        UpdateShieldTimer();
        UpdateSpeedBoostTimer();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        HandleCollision(other.tag, other.gameObject);
    }

    private void HandleCollision(string tag, GameObject obj)
    {
        switch (tag)
        {
            case "Obstacle":
                if (!isShieldActive && !isSpeedBoostActive)
                    TriggerGameOver();
                else
                    Debug.Log("Protected by shield/speedboost!");
                break;

            case "Ground":
            case "Ceiling":
                TriggerGameOver();
                break;

            case "PowerUp":
                ApplyPowerUp(obj.GetComponent<PowerUp>());
                Destroy(obj);
                break;
        }
    }

    private void UpdateShieldTimer()
    {
        if (!isShieldActive) return;

        shieldDurationRemaining -= Time.deltaTime;
        if (shieldDurationRemaining <= 0f)
        {
            DeactivateShield();
        }
    }

    private void UpdateSpeedBoostTimer()
    {
        if (!isSpeedBoostActive) return;

        speedBoostDurationRemaining -= Time.deltaTime;
        if (speedBoostDurationRemaining <= 0f)
        {
            DeactivateSpeedBoost();
        }
    }

    public void ActivateSpeedBoost(float duration)
    {
        Debug.Log("Game Speed boost activated!");
        GameManager.Instance.BoostGameSpeed(speedBoostMultiplier);

        isSpeedBoostActive = true;
        speedBoostDurationRemaining = duration;

        if (speedBoostParticles) speedBoostParticles.Play();
    }

    public void DeactivateSpeedBoost()
    {
        GameManager.Instance.ResetGameSpeed();

        isSpeedBoostActive = false;
        Debug.Log("Game Speed boost deactivated!");

        if (speedBoostParticles) speedBoostParticles.Stop();
    }

    private void ApplyPowerUp(PowerUp powerUp)
    {
        if (!powerUp) return;

        switch (powerUp.powerUpType)
        {
            case PowerUp.PowerUpType.Shield:
                ActivateShield(powerUp.duration);
                break;

            case PowerUp.PowerUpType.SpeedBoost:
                ActivateSpeedBoost(powerUp.duration);
                break;
        }

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddPowerUpPoints();
        }
    }

    public void ActivateShield(float duration)
    {
        isShieldActive = true;
        shieldDurationRemaining = duration;
        if (shieldVisual) shieldVisual.SetActive(true);
    }

    public void DeactivateShield()
    {
        isShieldActive = false;
        shieldDurationRemaining = 0f;
        if (shieldVisual) shieldVisual.SetActive(false);
    }



    /// <summary>
    /// This method triggers the game over sequence.
    /// It activates the game over screen and pauses the game by setting the time scale to 0.
    /// </summary>
    private void TriggerGameOver()
    {
        Debug.Log("Player died!");
        snapshot.TransitionTo(0);

        // FALSCHE REIHENFOLGE:
        // SceneManager.LoadScene("GameOverScene");
        // ScoreManager.Instance.GameOver();

        // RICHTIGE REIHENFOLGE:
        ScoreManager.Instance.GameOver();
        // Die GameOver-Methode ruft bereits SceneManager.LoadScene auf
    }


}
