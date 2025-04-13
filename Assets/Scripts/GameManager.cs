using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Geschwindigkeits-Einstellungen")]
    [SerializeField] private float initialMoveSpeed = 5f;
    [SerializeField] private float speedIncreaseRate = 0.5f;
    [SerializeField] private float maxMoveSpeed = 20f;

    public float InitialMoveSpeed => initialMoveSpeed;
    public float CurrentMoveSpeed { get; private set; }
    private float baseMoveSpeed;
    private bool isSpeedBoosted = false;

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
        IncreaseSpeed();

        if (Time.frameCount % 300 == 0)
        {
            Debug.Log($"Aktuelle Geschwindigkeit: {CurrentMoveSpeed}");
        }
    }

    public void ResetSpeed()
    {
        CurrentMoveSpeed = initialMoveSpeed;
        baseMoveSpeed = initialMoveSpeed;
        isSpeedBoosted = false;
    }

    public void IncreaseSpeed()
    {
        float acceleration = speedIncreaseRate * Time.deltaTime;

        float gameTimeMultiplier = Mathf.Clamp(Time.timeSinceLevelLoad / 60f, 1f, 3f);
        acceleration *= gameTimeMultiplier;

        CurrentMoveSpeed = Mathf.Min(CurrentMoveSpeed + acceleration, maxMoveSpeed);

        if (!isSpeedBoosted)
        {
            baseMoveSpeed = CurrentMoveSpeed;
        }
    }

    public void BoostGameSpeed(float multiplier)
    {
        if (!isSpeedBoosted)
        {
            baseMoveSpeed = CurrentMoveSpeed;
            isSpeedBoosted = true;
        }

        CurrentMoveSpeed = baseMoveSpeed * multiplier;

        //TODO: Überprüfen ob das noch benötigt wird
        // ParallaxEffect[] parallaxEffects = FindObjectsOfType<ParallaxEffect>();
        // foreach (var effect in parallaxEffects)
        // {
        //     effect.SetSpeed(CurrentMoveSpeed);
        // }

        Debug.Log($"Game speed boosted from {baseMoveSpeed} to {CurrentMoveSpeed}");
    }

    public void ResetGameSpeed()
    {
        CurrentMoveSpeed = baseMoveSpeed;
        isSpeedBoosted = false;

        //TODO: Überprüfen ob das noch benötigt wird
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
