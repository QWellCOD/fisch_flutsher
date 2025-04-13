using UnityEngine;

public class ObstacleMovement : MonoBehaviour
{
    [SerializeField] private bool useCustomSpeed = false;
    [SerializeField] [Range(0.8f, 1.2f)] private float speedModifier = 1f;
    
    void Update()
    {
        float moveSpeed = GameManager.Instance.CurrentMoveSpeed;

        if (useCustomSpeed)
        {
            moveSpeed *= speedModifier;
        }

        transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);

        if (transform.position.x < -15f)
        {
            Destroy(gameObject);
        }
    }
}
