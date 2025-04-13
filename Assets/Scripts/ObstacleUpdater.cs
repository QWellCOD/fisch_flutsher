using UnityEngine;

public class ObstacleUpdater : MonoBehaviour
{
    private Rigidbody2D rb;
    private float currentSpeed;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void SetSpeed(float speed)
    {
        currentSpeed = speed;
    }

    void Update()
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector2.left * currentSpeed;
        }
    }
}
