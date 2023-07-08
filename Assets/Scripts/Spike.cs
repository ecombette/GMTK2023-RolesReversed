using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : MonoBehaviour
{
    [SerializeField] private PathFindingTargetReference _target;
    [SerializeField] Transform _spike;
    [SerializeField] List<float> _yPos;
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
        float startYPos = _yPos[_currentState];

        _currentState++;
        if (_currentState >= _yPos.Count)
            _currentState = 0;

        float endYPos = _yPos[_currentState];

        StartCoroutine(lerpSpike());
        IEnumerator lerpSpike()
        {
            float t = 0f;

            while (t < 1f)
            {
                t += Time.deltaTime;

                _spike.position = new Vector3(_spike.position.x, _yPos[_currentState], _spike.position.z);
                yield return null;
            }
        }
    }
}
