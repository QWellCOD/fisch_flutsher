using UnityEngine;

public class ObstacleMovement : MonoBehaviour
{
    // Option für unterschiedliche Geschwindigkeiten
    [SerializeField] private bool useCustomSpeed = false;
    [SerializeField] [Range(0.8f, 1.2f)] private float speedModifier = 1f;
    
    void Update()
    {
        // Bewegungsgeschwindigkeit direkt aus dem GameManager holen
        float moveSpeed = GameManager.Instance.CurrentMoveSpeed;
        
        // Optional: Variabilität hinzufügen (für verschiedene Hindernistypen)
        if (useCustomSpeed)
        {
            moveSpeed *= speedModifier;
        }
        
        // Bewege das Hindernis mit der globalen Spielgeschwindigkeit
        transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);

        // Entferne das Hindernis, wenn es weit genug links ist
        if (transform.position.x < -15f)
        {
            Destroy(gameObject);
        }
    }
}
