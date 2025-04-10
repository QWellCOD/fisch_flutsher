using UnityEngine;
using System.Collections.Generic;

public class ParallaxEffect : MonoBehaviour
{
    [Header("Scroll-Einstellungen")]
    [SerializeField] private float baseScrollSpeed = 5f; // Basis-Geschwindigkeit
    [SerializeField][Range(0f, 1f)] private float parallaxStrength = 1f;
    [SerializeField] private bool syncWithGameManager = true; // Automatische Synchronisierung

    [Header("Layout-Einstellungen")]
    [SerializeField] private float offscreenBuffer = 1.5f;
    [SerializeField] private bool enableOverlap = true;
    [SerializeField] private float overlapPercentage = 0.05f; // 5% Überlappung

    private float currentScrollSpeed;
    private float spriteWidth;
    private List<Transform> sprites = new List<Transform>();
    private float screenWidth;
    private float leftBoundary;
    private float rightBoundary;

    private void Awake()
    {
        // Basis-Initialwerte setzen
        currentScrollSpeed = baseScrollSpeed;

        // Sprites initialisieren
        InitializeSprites();
    }

    private void Start()
    {
        // Initialen Wert vom GameManager holen
        if (syncWithGameManager && GameManager.Instance != null)
        {
            currentScrollSpeed = GameManager.Instance.CurrentMoveSpeed * parallaxStrength;
        }
    }

    private void InitializeSprites()
    {
        // Alle Kind-Sprites sammeln
        for (int i = 0; i < transform.childCount; i++)
        {
            sprites.Add(transform.GetChild(i));
        }

        if (sprites.Count == 0)
        {
            Debug.LogError("Keine Sprites für Parallax-Effekt gefunden!");
            return;
        }

        // Breite des ersten Sprites ermitteln
        spriteWidth = sprites[0].GetComponent<SpriteRenderer>().bounds.size.x;

        // Kamera-Breite berechnen
        screenWidth = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, 0)).x -
                      Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0)).x;

        // Grenzen berechnen
        UpdateBoundaries();

        // Prüfen ob genug Sprites vorhanden sind
        EnsureEnoughSprites();

        // Sprites positionieren
        ArrangeSpritesWithBuffer();
    }

    private void UpdateBoundaries()
    {
        leftBoundary = Camera.main.transform.position.x - (screenWidth / 2) - (spriteWidth * 0.5f);
        rightBoundary = Camera.main.transform.position.x + (screenWidth / 2) + (spriteWidth * offscreenBuffer);
    }

    private void EnsureEnoughSprites()
    {
        float totalWidthNeeded = screenWidth + (2 * screenWidth * offscreenBuffer);
        int spritesNeeded = Mathf.CeilToInt(totalWidthNeeded / spriteWidth);

        while (sprites.Count < spritesNeeded)
        {
            Transform newSprite = Instantiate(sprites[0], transform);
            sprites.Add(newSprite);
        }

        Debug.Log($"Benötige {spritesNeeded} Sprites für den Hintergrund, {sprites.Count} vorhanden.");
    }

    private void ArrangeSpritesWithBuffer()
    {
        float startX = Camera.main.transform.position.x - (screenWidth / 2) - (spriteWidth * 0.5f);
        float overlap = enableOverlap ? spriteWidth * overlapPercentage : 0f;

        for (int i = 0; i < sprites.Count; i++)
        {
            sprites[i].position = new Vector3(
                startX + (i * (spriteWidth - overlap)),
                sprites[i].position.y,
                sprites[i].position.z
            );
        }
    }

    private void Update()
    {
        // Mit GameManager synchronisieren, falls gewünscht
        if (syncWithGameManager && GameManager.Instance != null)
        {
            // WICHTIGSTE ÄNDERUNG: Direkte Verwendung der GameManager-Geschwindigkeit
            // statt einen Ratio zu berechnen, der zu Abweichungen führen kann
            currentScrollSpeed = GameManager.Instance.CurrentMoveSpeed * parallaxStrength;
        }

        // Bewegungsgeschwindigkeit berechnen (FPS-unabhängig)
        float movement = currentScrollSpeed * Time.deltaTime;

        // Grenzen aktualisieren
        UpdateBoundaries();

        // Alle Sprites bewegen
        foreach (var sprite in sprites)
        {
            sprite.Translate(Vector3.left * movement);
            RepositionSpriteIfNeeded(sprite);
        }
    }

    private void RepositionSpriteIfNeeded(Transform sprite)
    {
        if (sprite.position.x + (spriteWidth * 0.5f) < leftBoundary)
        {
            // Finde die rechteste Position
            float rightmostX = -Mathf.Infinity;
            Transform rightmostSprite = null;

            foreach (var s in sprites)
            {
                if (s == sprite) continue;
                if (s.position.x > rightmostX)
                {
                    rightmostX = s.position.x;
                    rightmostSprite = s;
                }
            }

            if (rightmostSprite != null)
            {
                float overlap = enableOverlap ? spriteWidth * overlapPercentage : 0f;

                // Platziere das Sprite rechts vom rechtesten Sprite
                sprite.position = new Vector3(
                    rightmostSprite.position.x + (spriteWidth - overlap),
                    sprite.position.y,
                    sprite.position.z
                );
            }
        }
    }

    // Manuelle Steuerung der Scroll-Geschwindigkeit
    public void SetSpeed(float newSpeed)
    {
        // Direkte Setzung der Geschwindigkeit ohne weitere Multiplikation
        currentScrollSpeed = newSpeed * parallaxStrength;
    }

    // Speed-Boost anwenden
    public void ApplySpeedMultiplier(float multiplier)
    {
        // Direkte Multiplikation der Basis-Geschwindigkeit
        currentScrollSpeed = GameManager.Instance.CurrentMoveSpeed * parallaxStrength;
        syncWithGameManager = true; // Automatische Synchronisierung beibehalten
    }

    // Zur Basis-Geschwindigkeit zurückkehren
    public void ResetToBaseSpeed()
    {
        // Synchronisierung mit dem GameManager wiederherstellen
        syncWithGameManager = true;
        currentScrollSpeed = GameManager.Instance.CurrentMoveSpeed * parallaxStrength;
    }

    // Debug-Visualisierung
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(
            new Vector3(leftBoundary, transform.position.y - 5, 0),
            new Vector3(leftBoundary, transform.position.y + 5, 0)
        );

        Gizmos.color = Color.green;
        Gizmos.DrawLine(
            new Vector3(rightBoundary, transform.position.y - 5, 0),
            new Vector3(rightBoundary, transform.position.y + 5, 0)
        );
    }
}
