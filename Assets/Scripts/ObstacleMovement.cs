using UnityEngine;

public class ObstacleMovement : MonoBehaviour
{
    void Update()
    {
        GameManager.Instance.IncreaseSpeed();
        transform.Translate(Vector3.left * GameManager.Instance.CurrentMoveSpeed * Time.deltaTime);

        if (transform.position.x < -15f)
        {
            Destroy(gameObject);
        }
    }
}
