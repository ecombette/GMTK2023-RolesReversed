using System.Collections;
using UnityEngine;

public class ChestController : MonoBehaviour
{
    [SerializeField]
    [Range(0.1f, 1f)]
    private float _inputSensitivity = 0.15f;
    [SerializeField][Range(0.1f, 2f)]
    private float _movementCooldown = 0.5f;

    private float _movementTimer = 0f;

    [SerializeField] float _movementSpeed = 3f;
    [SerializeField] AnimationCurve _movementCurve;
    [SerializeField] AnimationCurve _rotationCurve;

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
        Vector3 startPos = transform.position;
        Vector3 targetPos = transform.position += movement;

        Quaternion startRot = transform.rotation;
        Quaternion targetRot = Quaternion.identity /*Vector3.Cross(movement, Vector3.up) * 90f*/;

        StartCoroutine(lerpMovement());
        IEnumerator lerpMovement()
        {
            float t = 0f;

            while(t < 1f)
            {
                t += Time.deltaTime * _movementSpeed;

                transform.position = Vector3.Lerp(startPos, targetPos, _movementCurve.Evaluate(t));
                transform.rotation = Quaternion.Slerp(startRot, targetRot, _rotationCurve.Evaluate(t));
                yield return null;
            }
        }


        //transform.Rotate(Vector3.Cross(movement, Vector3.up), 90f, Space.World);
    }
}
