using UnityEngine;

public class PathFindingTarget : MonoBehaviour
{
    [SerializeField]
    private PathFindingTargetReference _targetReference;
    [SerializeField]
    private GridReference _gridReference;
    [SerializeField]
    private Node _currentNode;

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

    public void SetTargetMove()
    {
        //TODO
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
