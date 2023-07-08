using UnityEngine;

public class GridPathfinding : MonoBehaviour
{
    [SerializeField]
    private GridReference _gridReference;

    private PathFindingTargetReference _currentTarget;

    public void FindPath(PathFindingTargetReference targetReference)
    {
        if(_currentTarget != null && _currentTarget != targetReference)
            _currentTarget.ResetTarget();

        _currentTarget = targetReference;
        //TODO 0:)
    }

    public Vector3 GetNextPosition()
    {
        //TODO 0:)
        return Vector3.zero;
    }

    private float heuristic(Vector3 a, Vector3 b)
    {
        // Using a Manhattan distance heuristic here because the agent
        // can move in 4 directions in the grid
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }
}
