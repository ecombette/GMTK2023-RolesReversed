using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : MonoBehaviour
{
    [SerializeField] Node _node;
    [SerializeField] private PathFindingTargetReference _target;
    [SerializeField] Collider _playerDetectionCollider;

    [Header("Animation")]
    [SerializeField] Transform _spike;
    [SerializeField] List<float> _yPos;
    [SerializeField][Range(0f, 1f)]
    private float _movementDuration = .5f;
    [SerializeField]
    private AnimationCurve _movementCurve;
    [SerializeField][Range(0f, 1f)]
    private float _triggerEnablingThreshold = .5f;

    int _currentState = 0;

    private void Start()
    {
        if (_target)
        {
            var targetReference = _target.Reference;
            if (targetReference == null)
                Logger.LogError("No target referenced in asset yet, won't subscribe to move events");
            else
            {
                targetReference.OnTargetMoved += NextMove;
                targetReference.OnTargetMoveAttempt += NextMove;
            }
        }
    }

    public void NextMove()
    {
        _playerDetectionCollider.enabled = false;

        var startPos = _spike.position;
        startPos.y = _yPos[_currentState++];

        if (_currentState >= _yPos.Count)
            _currentState = 0;

        var endPos = startPos;
        endPos.y = _yPos[_currentState];
        _node.UpdateCost(_currentState == 0 ? 1 : 50);

        if (_currentState == _yPos.Count - 1)
            StartCoroutine(lerpDeadlySpike());
        else
            StartCoroutine(lerpSpike());

        IEnumerator lerpSpike()
        {
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / _movementDuration;

                _spike.position = Vector3.Lerp(startPos, endPos, _movementCurve.Evaluate(t));
                yield return null;
            }

            _spike.position = endPos;
        }

        IEnumerator lerpDeadlySpike()
        {
            float t = 0f;
            while (t < _triggerEnablingThreshold)
            {
                t += Time.deltaTime / _movementDuration;
                _spike.position = Vector3.Lerp(startPos, endPos, _movementCurve.Evaluate(t));

                yield return null;
            }

            _playerDetectionCollider.enabled = true;

            while (t < 1f)
            {
                t += Time.deltaTime / _movementDuration;
                _spike.position = Vector3.Lerp(startPos, endPos, _movementCurve.Evaluate(t));

                yield return null;
            }

            _spike.position = endPos;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.GameOver();
            _playerDetectionCollider.enabled = false;
        }
    }
}
