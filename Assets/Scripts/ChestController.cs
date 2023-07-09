using System.Collections;
using UnityEngine;

public class ChestController : MonoBehaviour
{
    [SerializeField]
    private Collider _chestTrigger;

    [Header("Input")]
    [SerializeField]
    [Range(0.1f, 1f)]
    private float _inputSensitivity = 0.15f;
    [SerializeField]
    [Range(0.1f, 2f)]
    private float _movementCooldown = 0.5f;
    [SerializeField]
    private PathFindingTarget _aiTargetManager;

    [Header("Movement")]
    [SerializeField] Transform _chest;
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
    private readonly int HASH_UP = Animator.StringToHash("XAngle");
    [SerializeField] Transform _chestDead;
    [SerializeField] GameObject _groundCollider;

    private float _movementTimer = 0f;

    [Header("Audio")]
    [SerializeField] AudioSource _source;
    [SerializeField] AudioClip _moveSFX;
    [SerializeField] AudioClip _cantMoveSFX;
    [SerializeField] AudioClip _dieSFX;
    [SerializeField] AudioClip _eatGoldSFX;

    private void OnEnable()
    {
        if (_animator)
            _animator.SetFloat(HASH_UP, 1);
    }

    private void Start()
    {
        var gameManager = GameManager.Instance;
        if (gameManager != null)
        {
            gameManager.SubscribeToLevelCompleted(() =>
            {
                _chestTrigger.enabled = false;
                enabled = false;
            });
            gameManager.SubscribeToGameOver(KillChest);
        }
        else
            Logger.LogError("No game manager found, chest can't subscribe to game events", this);
    }

    private void Update()
    {
        if (_movementTimer <= 0f)
        {
            float horizontalAxis = Input.GetAxis("Horizontal");
            float verticalAxis = Input.GetAxis("Vertical");
            Direction direction = Direction.NONE;
            if (Mathf.Abs(horizontalAxis) >= _inputSensitivity)
            {
                if (horizontalAxis > 0f)
                    direction = Direction.RIGHT;
                else
                    direction = Direction.LEFT;

                _movementTimer = _movementCooldown;
            }
            else if (Mathf.Abs(verticalAxis) >= _inputSensitivity)
            {
                if (verticalAxis > 0f)
                    direction = Direction.UP;
                else
                    direction = Direction.DOWN;

                _movementTimer = _movementCooldown;
            }

            tryToMove(direction);
        }
        else
            _movementTimer -= Time.deltaTime;
    }

    private void tryToMove(Direction direction)
    {
        if (direction == Direction.NONE)
            return;

        if (_aiTargetManager == null || _aiTargetManager.TryTargetMove(direction))
            move(direction);
        else
            cantMove();
    }

    private void move(Direction direction)
    {
        playSound(_moveSFX);

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
                Quaternion newRot = Quaternion.Slerp(startRot, targetRot, _rotationCurve.Evaluate(t));
                _chest.rotation = newRot;
                _chestDead.rotation = newRot;

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

        playSound(_cantMoveSFX);

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

    [ContextMenu("Kill Chest")]
    public void KillChest()
    {
        _groundCollider.SetActive(true);
        _chest.gameObject.SetActive(false);
        _chestDead.gameObject.SetActive(true);
        playSound(_dieSFX);
        this.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("GoldPile"))
            playSound(_eatGoldSFX);
    }

    private void playSound(AudioClip clip)
    {
        if (_source && clip)
        {
            _source.clip = clip;
            _source.Play();
        }
    }

#if UNITY_EDITOR
    [ContextMenu("Simulate LEFT")]
    private void editorSimulateLeft()
    {
        tryToMove(Direction.LEFT);
    }

    [ContextMenu("Simulate RIGHT")]
    private void editorSimulateRight()
    {
        tryToMove(Direction.RIGHT);
    }

    [ContextMenu("Simulate UP")]
    private void editorSimulateUp()
    {
        tryToMove(Direction.UP);
    }

    [ContextMenu("Simulate DOWN")]
    private void editorSimulateDown()
    {
        tryToMove(Direction.DOWN);
    }
#endif
}
