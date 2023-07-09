using System.Collections;
using UnityEngine;
using GD.MinMaxSlider;

public class EnemyController : MonoBehaviour
{
    [SerializeField]
    private GridPathfindingAgent _pathfindingManager;
    [SerializeField]
    private PathFindingTargetReference _target;
    [SerializeField]
    private Collider _playerDetectionCollider;

    [Header("Animations")]
    [SerializeField] Animator _animator;
    static int HASH_ATTACK = Animator.StringToHash("Attack");

    [SerializeField] float _movementSpeed = 5f;
    [SerializeField] AnimationCurve _movementCurve;
    [SerializeField] AnimationCurve _movementYCurve;
    [SerializeField] AnimationCurve _rotationYCurve;
    [SerializeField] AnimationCurve _scaleCurveX;
    [SerializeField] AnimationCurve _scaleCurveY;
    [SerializeField] AnimationCurve _scaleCurveZ;
    Coroutine _lerpScaleCoroutine;

    [Header("Audio")]
    [SerializeField] AudioSource _source;
    [SerializeField] AudioClip _kinghtMoveClip;
    [SerializeField, MinMaxSlider(0f, 2f)] Vector2 _pitchRandom = new Vector2(-0.5f,0.5f);
    [SerializeField] AudioClip _knightAttackClip;

    private void Start()
    {
        if(_target)
        {
            var targetReference = _target.Reference;
            if(targetReference == null)
                Logger.LogError("No target referenced in asset yet, won't subscribe to move events");
            else
            {
                UpdatePath();

                targetReference.OnTargetMoved += NextUpdatedMove;
                targetReference.OnTargetMoveAttempt += NextMove;
            }
        }
    }

    private void OnDestroy()
    {
        if (_target)
            _target.ResetTarget();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Attack();
            _playerDetectionCollider.enabled = false;
        }
    }

    [ContextMenu("Update Path")]
    public void UpdatePath()
    {
        _pathfindingManager.FindPath(_target);
    }

    [ContextMenu("Next Move")]
    public void NextMove()
    {
        var nextNode = _pathfindingManager.PeekNextNode();
        if(nextNode == null)
        {
            Logger.LogWarning("No next node found, can't move");
            return;
        }
        if(_pathfindingManager.IsTargetNode(nextNode))
        {
            // If the knight is facing the target node, do not move
            if (Vector3.Dot(transform.forward, nextNode.transform.position - transform.position) > 0.9f)
                return;

            move(transform.position, nextNode.transform.position);
            return;
        }

        move(_pathfindingManager.GetNextPosition(), _pathfindingManager.PeekNextPosition());
    }

    public void NextUpdatedMove()
    {
        var nextNode = _pathfindingManager.PeekNextNode();
        if (nextNode == null)
        {
            Logger.LogWarning("No next node found, can't move");
            return;
        }
        if (_pathfindingManager.IsTargetNode(nextNode))
        {
            // If the knight is facing the target node, do not move
            if (Vector3.Dot(transform.forward, nextNode.transform.position - transform.position) > 0.9f)
                return;

            move(transform.position, nextNode.transform.position);
            return;
        }

        var nextPosition = _pathfindingManager.GetNextPosition();
        UpdatePath();

        move(nextPosition, _pathfindingManager.PeekNextPosition());
    }

    private void move(Vector3 nextPosition, Vector3 nextPrediction)
    {
        playSound(_kinghtMoveClip, Random.Range(_pitchRandom.x, _pitchRandom.y));

        Vector3 startPos = transform.position;
        Quaternion startRotation = transform.rotation;
        Vector3 targetForward = nextPrediction == nextPosition ? nextPosition - startPos :  nextPrediction - nextPosition;
        Quaternion targetRotation = Quaternion.LookRotation(targetForward);

        StartCoroutine(lerpMovement());
        IEnumerator lerpMovement()
        {
            _playerDetectionCollider.enabled = false;

            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * _movementSpeed;

                transform.position = Vector3.Lerp(startPos, new Vector3(nextPosition.x, _movementYCurve.Evaluate(t), nextPosition.z), _movementCurve.Evaluate(t));
                transform.rotation = Quaternion.Lerp(startRotation, targetRotation, _rotationYCurve.Evaluate(t));
                yield return null;
            }

            transform.position = nextPosition;
            transform.rotation = targetRotation;

            if (_lerpScaleCoroutine != null)
                StopCoroutine(_lerpScaleCoroutine);

            _lerpScaleCoroutine = StartCoroutine(lerpScale());

            _playerDetectionCollider.enabled = true;
        }
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

    [ContextMenu("Attack")]
    public void Attack()
    {
        playSound(_knightAttackClip, 1f);
        _animator.SetTrigger(HASH_ATTACK);
        GameManager.Instance.GameOver();
    }

    private void playSound(AudioClip clip, float pitchValue)
    {
        if (_source && clip)
        {
            _source.pitch = pitchValue;
            _source.clip = clip;
            _source.Play();
        }
    }
}
