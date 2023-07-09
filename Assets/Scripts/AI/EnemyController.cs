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
    
    [SerializeField] float _movementSpeed = 5f;
    [SerializeField] AnimationCurve _movementCurve;
    [SerializeField] AnimationCurve _movementYCurve;
    [SerializeField] AnimationCurve _rotationYCurve;
    [SerializeField] AnimationCurve _scaleCurveX;
    [SerializeField] AnimationCurve _scaleCurveY;
    [SerializeField] AnimationCurve _scaleCurveZ;
    Coroutine _lerpScaleCoroutine;

    private readonly int HASH_STATE = Animator.StringToHash("State");
    private readonly int HASH_JUMP = Animator.StringToHash("Jump");
    private readonly int HASH_ATTACK = Animator.StringToHash("Attack");

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
        move(_pathfindingManager.GetNextPosition(), _pathfindingManager.PeekNextPosition());
    }

    private void move(Vector3 nextPosition, Vector3 nextPrediction)
    {
        Vector3 startPos = transform.position;
        Quaternion startRotation = transform.rotation;
        Vector3 targetForward = nextPrediction == nextPosition ? nextPosition - startPos :  nextPrediction - nextPosition;
        Quaternion targetRotation = Quaternion.LookRotation(targetForward);

        StartCoroutine(lerpMovement());
        IEnumerator lerpMovement()
        {
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
