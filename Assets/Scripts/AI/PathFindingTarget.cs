using UnityEngine;
using UnityEngine.Events;

public class PathFindingTarget : MonoBehaviour
{
    [SerializeField]
    private PathFindingTargetReference _targetReference;
    [SerializeField]
    private GridReference _gridReference;
    [SerializeField]
    private Node _currentNode;

    public UnityAction OnTargetMoved, OnTargetMoveAttempt;

    public Node CurrentNode => _currentNode;

    private void Awake()
    {
        if (_targetReference == null)
        {
            Logger.LogWarning("No level grid reference in script, won't set reference");
            return;
        }

        _targetReference.Init(this);
    }

    private void OnDestroy()
    {
        _targetReference.ResetTarget();
    }

    public void ResetListeners()
    {
        OnTargetMoved = null;
        OnTargetMoveAttempt = null;
    }

    public void SetTargetMove(Direction direction)
    {
        if(_currentNode == null)
        {
            Logger.LogWarning("No current node, can't move");
            return;
        }

        var nextNode = _currentNode.GetNeighbour(direction);
        if(nextNode == null)
        {
            Logger.Log($"Node {_currentNode} neighbouring node to its {direction}, won't move");
            //TODO: boink contre un obstacle :)
            OnTargetMoveAttempt?.Invoke();
            return;
        }
        
        _currentNode = nextNode;
        OnTargetMoved?.Invoke();
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
