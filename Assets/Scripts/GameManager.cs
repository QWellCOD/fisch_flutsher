using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Geschwindigkeits-Einstellungen")]
    [SerializeField] private float initialMoveSpeed = 5f;
    [SerializeField] private float speedIncreaseRate = 0.5f; // Erhöht für schnellere Beschleunigung
    [SerializeField] private float maxMoveSpeed = 20f; // Maximale Geschwindigkeit

    // Öffentliche Property für den Zugriff auf initialMoveSpeed
    public float InitialMoveSpeed => initialMoveSpeed;

    public float CurrentMoveSpeed { get; private set; }
    private float baseMoveSpeed; // Basis-Geschwindigkeit
    private bool isSpeedBoosted = false; // Flag für aktiven Speed Boost

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

    void Update()
    {
        // Geschwindigkeit stetig erhöhen
        IncreaseSpeed();

        // Debug-Ausgabe (nur für Entwicklung)
        if (Time.frameCount % 300 == 0) // Alle ~5 Sekunden
        {
            Debug.Log($"Aktuelle Geschwindigkeit: {CurrentMoveSpeed}");
        }
    }

    public void ResetSpeed()
    {
        CurrentMoveSpeed = initialMoveSpeed;
        baseMoveSpeed = initialMoveSpeed; // Basis-Geschwindigkeit auch zurücksetzen
        isSpeedBoosted = false;
    }

    public void IncreaseSpeed()
    {
        // Verbesserte Geschwindigkeitserhöhung mit Zeitfaktor
        float acceleration = speedIncreaseRate * Time.deltaTime;

        // Optional: Beschleunigung mit der Zeit steigern
        float gameTimeMultiplier = Mathf.Clamp(Time.timeSinceLevelLoad / 60f, 1f, 3f);
        acceleration *= gameTimeMultiplier;

        // Geschwindigkeit erhöhen, aber nicht über Maximum
        CurrentMoveSpeed = Mathf.Min(CurrentMoveSpeed + acceleration, maxMoveSpeed);

        // Wenn kein Boost aktiv ist, auch die Basis-Geschwindigkeit aktualisieren
        if (!isSpeedBoosted)
        {
            baseMoveSpeed = CurrentMoveSpeed;
        }
    }

    // Aktiviert den SpeedBoost
    public void BoostGameSpeed(float multiplier)
    {
        // Nur wenn noch kein Boost aktiv ist, die aktuelle Geschwindigkeit als Basis speichern
        if (!isSpeedBoosted)
        {
            // Basis-Geschwindigkeit speichern (vor dem Boost)
            baseMoveSpeed = CurrentMoveSpeed;
            isSpeedBoosted = true;
        }

        // Geschwindigkeit mit Multiplikator erhöhen
        CurrentMoveSpeed = baseMoveSpeed * multiplier;

        // Parallax-Effekte anpassen
        ParallaxEffect[] parallaxEffects = FindObjectsOfType<ParallaxEffect>();
        foreach (var effect in parallaxEffects)
        {
            effect.SetSpeed(CurrentMoveSpeed);
        }

        Debug.Log($"Game speed boosted from {baseMoveSpeed} to {CurrentMoveSpeed}");
    }

    // Deaktiviert den SpeedBoost
    public void ResetGameSpeed()
    {
        // Zurück zur Basis-Geschwindigkeit (vor dem Boost)
        CurrentMoveSpeed = baseMoveSpeed;
        isSpeedBoosted = false;

        // Parallax-Effekte zurücksetzen
        ParallaxEffect[] parallaxEffects = FindObjectsOfType<ParallaxEffect>();
        foreach (var effect in parallaxEffects)
        {
            effect.SetSpeed(baseMoveSpeed);
        }

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
