using UnityEngine;

public class BubbleEffectIntensifier : MonoBehaviour
{
    [Header("Zielsystem")]
    [SerializeField] private ParticleSystem bubbleSystem;

    [Header("Verstärkungsparameter")]
    [SerializeField] private float intensificationDuration = 60f; // Dauer in Sekunden bis zur maximalen Intensität
    [SerializeField] private float startVelocityX = -1f; // Anfangsgeschwindigkeit
    [SerializeField] private float maxVelocityX = -5f; // Maximale Geschwindigkeit
    [SerializeField] private float startEmissionRate = 5f; // Anfängliche Blasen pro Sekunde
    [SerializeField] private float maxEmissionRate = 15f; // Maximale Blasen pro Sekunde
    [SerializeField] private float startStretchFactor = 1f; // Anfängliche Streckung
    [SerializeField] private float maxStretchFactor = 3f; // Maximale Streckung

    [Header("Verlaufskurve")]
    [SerializeField] private AnimationCurve intensityCurve = AnimationCurve.EaseInOut(0, 0, 1, 1); // Wie sich der Effekt mit der Zeit verstärkt

    // Private Variablen
    private float timer = 0f;
    private ParticleSystem.VelocityOverLifetimeModule velocityModule;
    private ParticleSystem.EmissionModule emissionModule;
    private ParticleSystemRenderer particleRenderer;
    private bool isInitialized = false;

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        if (bubbleSystem == null)
        {
            bubbleSystem = GetComponent<ParticleSystem>();
            if (bubbleSystem == null)
            {
                Debug.LogError("Kein Particle System gefunden! Bitte zuweisen oder an GameObject mit ParticleSystem anhängen.");
                enabled = false;
                return;
            }
        }

        velocityModule = bubbleSystem.velocityOverLifetime;
        emissionModule = bubbleSystem.emission;
        particleRenderer = bubbleSystem.GetComponent<ParticleSystemRenderer>();

        // Anfangswerte setzen
        SetVelocityX(startVelocityX);
        SetEmissionRate(startEmissionRate);
        SetStretchFactor(startStretchFactor);

        isInitialized = true;
    }

    private void Update()
    {
        if (!isInitialized) Initialize();

        // Timer aktualisieren
        timer += Time.deltaTime;
        float normalizedTime = Mathf.Clamp01(timer / intensificationDuration);

        // Intensität basierend auf der Kurve berechnen
        float intensity = intensityCurve.Evaluate(normalizedTime);

        // Parameter interpolieren und anwenden
        float currentVelocityX = Mathf.Lerp(startVelocityX, maxVelocityX, intensity);
        float currentEmissionRate = Mathf.Lerp(startEmissionRate, maxEmissionRate, intensity);
        float currentStretchFactor = Mathf.Lerp(startStretchFactor, maxStretchFactor, intensity);

        // Auf Particle System anwenden
        SetVelocityX(currentVelocityX);
        SetEmissionRate(currentEmissionRate);
        SetStretchFactor(currentStretchFactor);
    }

    private void SetVelocityX(float value)
    {
        if (!velocityModule.enabled)
            velocityModule.enabled = true;

        velocityModule.x = value;
    }

    private void SetEmissionRate(float value)
    {
        emissionModule.rateOverTime = value;
    }

    private void SetStretchFactor(float value)
    {
        if (particleRenderer.renderMode != ParticleSystemRenderMode.Stretch)
            particleRenderer.renderMode = ParticleSystemRenderMode.Stretch;

        particleRenderer.velocityScale = value;
    }

    // Optional: Methode zum manuellen Zurücksetzen des Effekts
    public void ResetIntensification()
    {
        timer = 0f;
        SetVelocityX(startVelocityX);
        SetEmissionRate(startEmissionRate);
        SetStretchFactor(startStretchFactor);
    }

    // Optional: Methode zum sofortigen Setzen der maximalen Intensität
    public void SetMaxIntensity()
    {
        timer = intensificationDuration;
        SetVelocityX(maxVelocityX);
        SetEmissionRate(maxEmissionRate);
        SetStretchFactor(maxStretchFactor);
    }
}
