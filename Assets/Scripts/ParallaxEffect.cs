using UnityEngine;
using System.Collections.Generic;

public class ParallaxEffect : MonoBehaviour
{
    [SerializeField] private float scrollSpeed = 5f;
    [SerializeField][Range(0f, 1f)] private float parallaxStrength = 1f;

    // Zusätzlicher Puffer außerhalb des sichtbaren Bereichs (in Bildschirmbreiten)
    [SerializeField] private float offscreenBuffer = 1.5f;

    private float spriteWidth;
    private List<Transform> sprites = new List<Transform>();
    private float screenWidth;
    private float leftBoundary;
    private float rightBoundary;

    private void Awake()
    {
        // In Awake statt Start, damit es vor dem ersten Frame ausgeführt wird
        InitializeSprites();
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

        // Kamera-Breite in Weltkoordinaten berechnen
        screenWidth = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, 0)).x -
                      Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0)).x;

        // Grenzen berechnen mit Puffer
        leftBoundary = Camera.main.transform.position.x - (screenWidth / 2) - (spriteWidth * 0.5f);
        rightBoundary = Camera.main.transform.position.x + (screenWidth / 2) + (spriteWidth * offscreenBuffer);

        // Prüfen ob genug Sprites vorhanden sind, um die Szene + Puffer zu füllen
        EnsureEnoughSprites();

        // Sprites positionieren
        ArrangeSpritesWithBuffer();
    }

    private void EnsureEnoughSprites()
    {
        // Berechnen, wie viele Sprites wir benötigen, um den Bildschirm + Puffer zu füllen
        float totalWidthNeeded = screenWidth + (2 * screenWidth * offscreenBuffer);
        int spritesNeeded = Mathf.CeilToInt(totalWidthNeeded / spriteWidth);

        while (sprites.Count < spritesNeeded)
        {
            // Klone das erste Sprite und füge es zur Liste hinzu
            Transform newSprite = Instantiate(sprites[0], transform);
            sprites.Add(newSprite);
        }

        Debug.Log($"Benötige {spritesNeeded} Sprites für den Hintergrund, {sprites.Count} vorhanden.");
    }

    private void ArrangeSpritesWithBuffer()
    {
        // Startposition ist links vom sichtbaren Bereich
        float startX = Camera.main.transform.position.x - (screenWidth / 2) - (spriteWidth * 0.5f);

        // Sprites von links nach rechts anordnen, mit Puffer außerhalb des sichtbaren Bereichs
        for (int i = 0; i < sprites.Count; i++)
        {
            sprites[i].position = new Vector3(
                startX + (i * spriteWidth),
                sprites[i].position.y,
                sprites[i].position.z
            );
        }

        float overlap = spriteWidth * 0.05f; // 5% Überlappung

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
        // Bewegungsgeschwindigkeit berechnen
        float movement = scrollSpeed * parallaxStrength * Time.deltaTime;

        // fps unabhängig
        float smoothDeltaTime = Time.smoothDeltaTime;


        // Grenzen aktualisieren basierend auf der aktuellen Kameraposition
        leftBoundary = Camera.main.transform.position.x - (screenWidth / 2) - (spriteWidth * 0.5f);
        rightBoundary = Camera.main.transform.position.x + (screenWidth / 2) + (spriteWidth * offscreenBuffer);

        // Alle Sprites bewegen
        foreach (var sprite in sprites)
        {
            sprite.Translate(Vector3.left * movement);
            RepositionSpriteIfNeeded(sprite);
        }

    }

    private void RepositionSpriteIfNeeded(Transform sprite)
    {
        // Wenn das Sprite zu weit links ist (komplett außerhalb des sichtbaren Bereichs + Puffer)
        if (sprite.position.x + (spriteWidth * 0.5f) < leftBoundary)
        {
            // Finde die rechteste Position aller Sprites
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
                // Platziere das Sprite direkt rechts vom aktuell rechtesten Sprite
                sprite.position = new Vector3(
                    rightmostSprite.position.x + spriteWidth,
                    sprite.position.y,
                    sprite.position.z
                );

                // Debug-Log, um die Repositionierung zu verfolgen
                Debug.Log($"Sprite repositioniert: Von {leftBoundary} nach {sprite.position.x}");
            }
        }
    }

    public void SetSpeed(float newSpeed)
    {
        scrollSpeed = newSpeed;
    }

    // Nur für Debug-Zwecke
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        // Grenzen visuell darstellen
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
