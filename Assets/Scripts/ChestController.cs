using UnityEngine;

public class ChestController : MonoBehaviour
{
    [SerializeField]
    [Range(0.1f, 1f)]
    private float _inputSensitivity = 0.15f;
    [SerializeField][Range(0.1f, 2f)]
    private float _movementCooldown = 0.5f;

    private float _movementTimer = 0f;

    private void Update()
    {
        if (_movementTimer <= 0f)
        {
            float horizontalAxis = Input.GetAxis("Horizontal");
            float verticalAxis = Input.GetAxis("Vertical");
            if(Mathf.Abs(horizontalAxis) >= _inputSensitivity)
            {
                if (horizontalAxis > 0f)
                    move(Vector3.right);
                else
                    move(Vector3.left);

                _movementTimer = _movementCooldown;
            }
            else if(Mathf.Abs(verticalAxis) >= _inputSensitivity)
            {
                if (verticalAxis > 0f)
                    move(Vector3.forward);
                else
                    move(-Vector3.forward);

                _movementTimer = _movementCooldown;
            }
        }
        else
            _movementTimer -= Time.deltaTime;
    }

    private void move(Vector3 movement)
    {
        transform.position += movement;
        transform.Rotate(Vector3.Cross(movement, Vector3.up), 90f, Space.World);
    }
}
