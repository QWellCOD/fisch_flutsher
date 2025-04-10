using UnityEngine;
using System.Collections.Generic;

public class ParallaxEffect : MonoBehaviour
{
    [SerializeField] private float scrollSpeed = 5f;
    [SerializeField][Range(0f, 1f)] private float parallaxStrength = 1f;

    private float spriteWidth;
    private List<Transform> sprites = new List<Transform>();
    private float screenWidth;

    private void Start()
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

        // Sprites nebeneinander positionieren
        ArrangeSprites();
    }

    private void ArrangeSprites()
    {
        // Sprites in einer Reihe positionieren
        for (int i = 0; i < sprites.Count; i++)
        {
            sprites[i].position = new Vector3(
                sprites[0].position.x + i * spriteWidth,
                sprites[i].position.y,
                sprites[i].position.z
            );
        }
    }

    private void Update()
    {
        // Bewegungsgeschwindigkeit berechnen
        float movement = scrollSpeed * parallaxStrength * Time.deltaTime;

        // Alle Sprites bewegen
        foreach (var sprite in sprites)
        {
            sprite.Translate(Vector3.left * movement);

            // Wenn Sprite am linken Rand verschwindet, rechts wieder einfügen
            RepositionSpriteIfNeeded(sprite);
        }
    }

    private void RepositionSpriteIfNeeded(Transform sprite)
    {
        // Wenn das Sprite komplett aus dem Bild links ist
        if (sprite.position.x + spriteWidth / 2 < Camera.main.transform.position.x - screenWidth / 2)
        {
            // Finde die rechteste Position aller Sprites
            float rightmostX = -Mathf.Infinity;
            foreach (var s in sprites)
            {
                if (s == sprite) continue; // Aktuelles Sprite überspringen
                if (s.position.x > rightmostX)
                {
                    rightmostX = s.position.x;
                }
            }

            // Platziere das Sprite rechts vom aktuell rechtesten Sprite
            sprite.position = new Vector3(
                rightmostX + spriteWidth,
                sprite.position.y,
                sprite.position.z
            );
        }
    }

    public void SetSpeed(float newSpeed)
    {
        scrollSpeed = newSpeed;
    }

    private void AdjustSpriteAlpha()
    {
        // Transparenten Übergang zwischen den Sprites erstellen
        foreach (var sprite in sprites)
        {
            SpriteRenderer renderer = sprite.GetComponent<SpriteRenderer>();
            // Hier könntest du Fading an den Rändern implementieren
            // renderer.material.SetFloat("_FadeEdge", 0.1f); // Benötigt einen speziellen Shader
        }
    }

    private void CreateEnoughSprites()
    {
        // Benötigte Anzahl an Sprites berechnen
        int neededSprites = Mathf.CeilToInt((screenWidth * 2) / spriteWidth) + 1;

        // Stelle sicher, dass wir genug Sprites haben
        if (sprites.Count < neededSprites)
        {
            int toAdd = neededSprites - sprites.Count;
            for (int i = 0; i < toAdd; i++)
            {
                // Erstelle Kopien des ersten Sprites
                Transform newSprite = Instantiate(sprites[0], transform);
                sprites.Add(newSprite);
            }
        }
    }
}
