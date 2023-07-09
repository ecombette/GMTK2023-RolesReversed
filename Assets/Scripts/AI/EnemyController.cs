using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField]
    private GridPathfindingAgent _pathfindingManager;
    [SerializeField]
    private PathFindingTargetReference _target;

    [Header("Animations")]
    [SerializeField] Animator _animator;
    [SerializeField] int _maxIdleState = 2;
    static int HASH_STATE = Animator.StringToHash("State");
    static int HASH_JUMP = Animator.StringToHash("Jump");
    static int HASH_ATTACK = Animator.StringToHash("Attack");

    [SerializeField] float _movementSpeed = 5f;
    [SerializeField] AnimationCurve _movementCurve;
    [SerializeField] AnimationCurve _movementYCurve;

    [SerializeField] AnimationCurve _scaleCurveX;
    [SerializeField] AnimationCurve _scaleCurveY;
    [SerializeField] AnimationCurve _scaleCurveZ;
    Coroutine _lerpScaleCoroutine;

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

                targetReference.OnTargetMoved += NextMove;
                targetReference.OnTargetMoved += UpdatePath;
                targetReference.OnTargetMoveAttempt += NextMove;
            }
        }
    }

    private void OnDestroy()
    {
        if(_target)
            _target.ResetTarget();
    }

    [ContextMenu("Update Path")]
    public void UpdatePath()
    {
        _pathfindingManager.FindPath(_target);
    }

    [ContextMenu("Next Move")]
    public void NextMove()
    {
        move(_pathfindingManager.GetNextPosition());
    }

    private void move(Vector3 nextPosition)
    {
        Vector3 startPos = transform.position;
        Vector3 targetPos = nextPosition;

        StartCoroutine(lerpMovement());
        IEnumerator lerpMovement()
        {
            float t = 0f;

            while (t < 1f)
            {
                t += Time.deltaTime * _movementSpeed;

                transform.position = Vector3.Lerp(startPos, new Vector3(targetPos.x, _movementYCurve.Evaluate(t), targetPos.z), _movementCurve.Evaluate(t));
                yield return null;
            }

            if (_lerpScaleCoroutine != null)
                StopCoroutine(_lerpScaleCoroutine);

            _lerpScaleCoroutine = StartCoroutine(lerpScale());
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
}
