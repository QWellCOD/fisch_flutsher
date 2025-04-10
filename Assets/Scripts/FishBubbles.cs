using UnityEngine;

public class FishBubbles : MonoBehaviour
{
    [Header("Referenzen")]
    [SerializeField] private GameObject fishObject;
    [SerializeField] private Sprite bubblesSprite;

    [Header("Blasen-Einstellungen")]
    [SerializeField] private float emissionRate = 8f;
    [SerializeField] private float bubbleLifetime = 4f;
    [SerializeField] private float bubbleSize = 0.6f;
    [SerializeField] private float spawnOffset = -0.5f;

    private ParticleSystem bubbleSystem;
    private bool isInitialized = false;

    void OnEnable()
    {
        // Initialisiere das Particle System beim Aktivieren des GameObjects
        InitializeParticleSystem();

        // Debug-Ausgabe, um zu überprüfen ob diese Methode aufgerufen wird
        Debug.Log("Blasen-Skript aktiviert!");
    }

    void InitializeParticleSystem()
    {
        if (isInitialized) return;

        // Particle System abrufen oder erstellen
        bubbleSystem = GetComponent<ParticleSystem>();
        if (bubbleSystem == null)
        {
            bubbleSystem = gameObject.AddComponent<ParticleSystem>();
            Debug.Log("Neues Particle System erstellt");
        }

        // Position setzen
        transform.position = new Vector3(
            fishObject.transform.position.x + spawnOffset,
            fishObject.transform.position.y,
            fishObject.transform.position.z - 0.1f  // Leicht vor dem Fisch für bessere Sichtbarkeit
        );

        // Haupteinstellungen
        var main = bubbleSystem.main;
        main.playOnAwake = true;  // Automatisch starten
        main.duration = 5.0f;
        main.loop = true;
        main.startLifetime = bubbleLifetime;
        main.startSpeed = 0.3f;
        main.startSize = bubbleSize;
        main.startRotation = 0f;  // Keine Rotation
        main.simulationSpace = ParticleSystemSimulationSpace.World;  // Wichtig für korrekte Positionierung
        main.gravityModifier = -0.05f;
        main.maxParticles = 50;   // Erhöht für mehr gleichzeitige Partikel

        // Emission konfigurieren
        var emission = bubbleSystem.emission;
        emission.enabled = true;
        emission.rateOverTime = emissionRate;

        // Form konfigurieren
        var shape = bubbleSystem.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.05f;

        // Renderer-Einstellungen
        var renderer = bubbleSystem.GetComponent<ParticleSystemRenderer>();
        renderer.renderMode = ParticleSystemRenderMode.Billboard;
        renderer.sortingOrder = 10;   // Höherer Wert = weiter vorne

        // Texture Sheet Animation für die 4 Blasenzustände
        var textureSheet = bubbleSystem.textureSheetAnimation;
        textureSheet.enabled = true;
        textureSheet.mode = ParticleSystemAnimationMode.Grid;
        textureSheet.numTilesX = 4;   // 4 Animationszustände
        textureSheet.numTilesY = 1;
        textureSheet.cycleCount = 1;  // Einmal durchlaufen

        // Animation über Zeit
        AnimationCurve frameCurve = new AnimationCurve();
        frameCurve.AddKey(0, 0);
        frameCurve.AddKey(1, 3);
        textureSheet.frameOverTime = new ParticleSystem.MinMaxCurve(1.0f, frameCurve);

        // Material erstellen
        Material bubbleMaterial = new Material(Shader.Find("Particles/Standard Unlit"));
        if (bubblesSprite != null)
        {
            bubbleMaterial.mainTexture = bubblesSprite.texture;
        }
        else
        {
            Debug.LogError("Kein Bubble-Sprite zugewiesen!");
        }
        renderer.material = bubbleMaterial;

        // Partikel explizit starten
        bubbleSystem.Clear();
        bubbleSystem.Play();

        isInitialized = true;
        Debug.Log("Particle System initialisiert und gestartet");
    }
}
