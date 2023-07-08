using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField]
    private GridPathfindingAgent _pathfindingManager;
    [SerializeField]
    private PathFindingTargetReference _target;

    private void Start()
    {
        if(_target)
        {
            var targetReference = _target.Reference;
            if(targetReference == null)
                Logger.LogError("No target referenced in asset yet, won't subscribe to move events");
            else
            {
                targetReference.OnTargetMoved += UpdatePath;
                targetReference.OnTargetMoved += NextMove;
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
        transform.position = _pathfindingManager.GetNextPosition();
    }
}
