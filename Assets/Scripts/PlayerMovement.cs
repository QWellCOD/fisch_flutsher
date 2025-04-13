using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float tiltAngle = 30f;
    public float tiltSpeed = 5f;

    private float currentRotation = 0f;

    void Update()
    {
        float verticalInput = 0;

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            verticalInput = 1;
        }

        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            verticalInput = -1;
        }

        transform.Translate(Vector2.up * verticalInput * moveSpeed * Time.deltaTime, Space.World);

        float targetRotation = 0f;

        if (verticalInput != 0)
        {

            targetRotation = verticalInput * tiltAngle;
        }

        currentRotation = Mathf.Lerp(currentRotation, targetRotation, tiltSpeed * Time.deltaTime);

        transform.rotation = Quaternion.Euler(0, 0, currentRotation);
    }
}
