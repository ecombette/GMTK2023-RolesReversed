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

        if(_gridReference.IsInitialized)
            RefreshCurrentNode();
        else
            _gridReference.SubscribeToInitialization(RefreshCurrentNode);
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

    public bool TryTargetMove(Direction direction)
    {
        if(_currentNode == null)
        {
            Logger.LogWarning("No current node, can't move");
            OnTargetMoveAttempt?.Invoke();
            return false;
        }

        var nextNode = _currentNode.GetNeighbour(direction);
        if(nextNode == null || !nextNode.IsWalkable)
        {
            Logger.Log($"Node {_currentNode} neighbouring node to its {direction}, won't move");
            OnTargetMoveAttempt?.Invoke();
            return false;
        }
        
        _currentNode = nextNode;
        OnTargetMoved?.Invoke();
        return true;
    }

    [ContextMenu("Refresh Current Node")]
    public void RefreshCurrentNode()
    {
        _gridReference.UnsubscribeFromInitialization(RefreshCurrentNode);

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

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }
}
