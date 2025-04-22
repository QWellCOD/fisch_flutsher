using UnityEngine;
using System.Collections.Generic;

public class ParallaxEffect : MonoBehaviour
{
    [Header("Scroll-Settings")]
    [SerializeField] private float baseScrollSpeed = 5f;
    [SerializeField][Range(0f, 1f)] private float parallaxStrength = 1f;
    [SerializeField] private bool syncWithGameManager = true;

    [Header("Layout-Settings")]
    [SerializeField] private float offscreenBuffer = 1.5f;
    [SerializeField] private bool enableOverlap = true;
    [SerializeField] private float overlapPercentage = 0.05f;

    private float currentScrollSpeed;
    private float spriteWidth;
    private List<Transform> sprites = new List<Transform>();
    private float screenWidth;
    private float leftBoundary;
    private float rightBoundary;

    private void Awake()
    {
        currentScrollSpeed = baseScrollSpeed;

        InitializeSprites();
    }

    private void Start()
    {
        if (syncWithGameManager && GameManager.Instance != null)
        {
            currentScrollSpeed = GameManager.Instance.CurrentMoveSpeed * parallaxStrength;
        }
    }

    private void InitializeSprites()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            sprites.Add(transform.GetChild(i));
        }

        if (sprites.Count == 0)
        {
            Debug.LogError("No sprites found for parallax effect!");
            return;
        }

        spriteWidth = sprites[0].GetComponent<SpriteRenderer>().bounds.size.x;

        screenWidth = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, 0)).x -
                      Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0)).x;

        UpdateBoundaries();

        EnsureEnoughSprites();

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

        Debug.Log($"Need {spritesNeeded} sprites for the background, {sprites.Count} available.");
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
        if (syncWithGameManager && GameManager.Instance != null)
        {
            currentScrollSpeed = GameManager.Instance.CurrentMoveSpeed * parallaxStrength;
        }

        float movement = currentScrollSpeed * Time.deltaTime;

        UpdateBoundaries();

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

                sprite.position = new Vector3(
                    rightmostSprite.position.x + (spriteWidth - overlap),
                    sprite.position.y,
                    sprite.position.z
                );
            }
        }
    }

    public void SetSpeed(float newSpeed)
    {
        currentScrollSpeed = newSpeed * parallaxStrength;
    }

    public void ApplySpeedMultiplier(float multiplier)
    {
        currentScrollSpeed = GameManager.Instance.CurrentMoveSpeed * parallaxStrength;
        syncWithGameManager = true;
    }

    public void ResetToBaseSpeed()
    {
        syncWithGameManager = true;
        currentScrollSpeed = GameManager.Instance.CurrentMoveSpeed * parallaxStrength;
    }

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
