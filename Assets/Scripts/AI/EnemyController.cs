using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField]
    private GridPathfinding _pathfindingManager;
    [SerializeField]
    private PathFindingTargetReference _target;

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
