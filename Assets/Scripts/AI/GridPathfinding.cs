using UnityEngine;

public class GridPathfinding : MonoBehaviour
{
    [SerializeField]
    private GridReference _gridReference;
    [SerializeField]
    private Node _currentNode;

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

#if UNITY_EDITOR
    [ContextMenu("Refresh Current Node")]
    public void EditorRefreshCurrentNode()
    {
        var gridNodes = _gridReference.Nodes;
        float closestNodeSqrDistance = Mathf.Infinity;
        foreach (var node in gridNodes)
        {
            var nodeSqrDistance = Vector3.SqrMagnitude(transform.position - node.transform.position);
            if (nodeSqrDistance < closestNodeSqrDistance)
            {
                _currentNode = node;
                closestNodeSqrDistance = nodeSqrDistance;
            }
        }

        UnityEditor.EditorUtility.SetDirty(this);
    }
#endif
}
