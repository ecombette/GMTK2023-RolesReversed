using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class ChestController : MonoBehaviour
{
    [SerializeField] Transform _chest;
    [SerializeField]
    [Range(0.1f, 1f)]
    private float _inputSensitivity = 0.15f;
    [SerializeField]
    [Range(0.1f, 2f)]
    private float _movementCooldown = 0.5f;

    [SerializeField]
    private UnityEvent<Direction> _onMove;

    private float _movementTimer = 0f;

    [SerializeField] float _movementSpeed = 5f;
    [SerializeField] AnimationCurve _movementCurve;
    [SerializeField] AnimationCurve _rotationCurve;
    [SerializeField] AnimationCurve _movementYCurve;

    [SerializeField] AnimationCurve _scaleCurveX;
    [SerializeField] AnimationCurve _scaleCurveY;
    [SerializeField] AnimationCurve _scaleCurveZ;
    Coroutine _lerpScaleCoroutine;

    [Header("Animation")]
    [SerializeField] Animator _animator;
    static int HASH_UP = Animator.StringToHash("XAngle");

    private void OnEnable()
    {
        if (_animator)
            _animator.SetFloat(HASH_UP, 1);
    }

    private void Update()
    {
        if (_movementTimer <= 0f)
        {
            float horizontalAxis = Input.GetAxis("Horizontal");
            float verticalAxis = Input.GetAxis("Vertical");
            if (Mathf.Abs(horizontalAxis) >= _inputSensitivity)
            {
                if (horizontalAxis > 0f)
                    move(Direction.RIGHT);
                else
                    move(Direction.LEFT);

                _movementTimer = _movementCooldown;
            }
            else if (Mathf.Abs(verticalAxis) >= _inputSensitivity)
            {
                if (verticalAxis > 0f)
                    move(Direction.UP);
                else
                    move(Direction.DOWN);

                _movementTimer = _movementCooldown;
            }
        }
        else
            _movementTimer -= Time.deltaTime;
    }

    private void move(Direction direction)
    {
        _onMove?.Invoke(direction);

        Vector3 movement = DirectionUtility.GetDirectionalMovement(direction);
        Vector3 startPos = transform.position;
        Vector3 targetPos = transform.position + movement;

        Quaternion startRot = _chest.rotation;
        Quaternion targetRot = Quaternion.AngleAxis(-90f, Vector3.Cross(movement, Vector3.up)) * startRot;

        StartCoroutine(lerpMovement());
        IEnumerator lerpMovement()
        {
            float t = 0f;

            while (t < 1f)
            {
                t += Time.deltaTime * _movementSpeed;

                transform.position = Vector3.Lerp(startPos, new Vector3(targetPos.x, _movementYCurve.Evaluate(t), targetPos.z), _movementCurve.Evaluate(t));
                _chest.rotation = Quaternion.Slerp(startRot, targetRot, _rotationCurve.Evaluate(t));

                if (_animator)
                    _animator.SetFloat(HASH_UP, Vector3.Dot(transform.up, _chest.up));

                yield return null;
            }

            if (_lerpScaleCoroutine != null)
                StopCoroutine(_lerpScaleCoroutine);

            _lerpScaleCoroutine = StartCoroutine(lerpScale());
        }
    }

    [ContextMenu("Can't Move")]
    public void cantMove()
    {
        if (_lerpScaleCoroutine != null)
            StopCoroutine(_lerpScaleCoroutine);

        _lerpScaleCoroutine = StartCoroutine(lerpScale());
    }

    IEnumerator lerpScale()
    {
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * _movementSpeed;

            transform.localScale = new Vector3(_scaleCurveX.Evaluate(t), _scaleCurveY.Evaluate(t), _scaleCurveZ.Evaluate(t));
            yield return null;
        }
    }
}
