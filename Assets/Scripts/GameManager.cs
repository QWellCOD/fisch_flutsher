using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private float initialMoveSpeed = 5f;
    [SerializeField] private float speedIncreaseRate = 0.1f;

    public float CurrentMoveSpeed { get; private set; }
    private float baseMoveSpeed; // Basis-Geschwindigkeit

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
        ResetSpeed();
    }

    public void ResetSpeed()
    {
        CurrentMoveSpeed = initialMoveSpeed;
        baseMoveSpeed = initialMoveSpeed; // Basis-Geschwindigkeit auch zuruecksetzen
    }

    public void IncreaseSpeed()
    {
        CurrentMoveSpeed += speedIncreaseRate * Time.deltaTime;

        // Wenn kein Boost aktiv ist, auch die Basis-Geschwindigkeit aktualisieren
        if (CurrentMoveSpeed <= baseMoveSpeed)
        {
            baseMoveSpeed = CurrentMoveSpeed;
        }
    }

    // NEUE METHODE: Aktiviert den SpeedBoost
    public void BoostGameSpeed(float multiplier)
    {
        // Aktuelle Geschwindigkeit als Basis speichern
        baseMoveSpeed = CurrentMoveSpeed;

        // Geschwindigkeit mit Multiplikator erh�hen
        CurrentMoveSpeed = baseMoveSpeed * multiplier;

        Debug.Log($"Game speed boosted from {baseMoveSpeed} to {CurrentMoveSpeed}");
    }

    // NEUE METHODE: Deaktiviert den SpeedBoost
    public void ResetGameSpeed()
    {
        CurrentMoveSpeed = baseMoveSpeed;
        Debug.Log($"Game speed reset to {baseMoveSpeed}");
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ResetSpeed();
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
