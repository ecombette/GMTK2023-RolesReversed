using System.Collections.Generic;
using UnityEngine;
using Priority_Queue;

public class GridPathfindingAgent : MonoBehaviour
{
    [SerializeField]
    private GridReference _gridReference;
    [SerializeField]
    private Node _currentNode;

    private PathFindingTargetReference _currentTarget;
    private readonly Dictionary<Node, Node> _cameFrom = new Dictionary<Node, Node>();
    private readonly Dictionary<Node, int> _costSoFar = new Dictionary<Node, int>();
    private readonly Dictionary<Node, Node> _currentPath = new Dictionary<Node, Node>();

    public void FindPath(PathFindingTargetReference targetReference)
    {
        if(targetReference == null || targetReference.Reference == null)
        {
            Logger.LogError("Trying to find path to empty target, aborting");
            return;
        }

        if (_currentTarget != null && _currentTarget != targetReference)
            _currentTarget.ResetTarget();

        _currentTarget = targetReference;
        var targetNode = targetReference.CurrentNode;
        _cameFrom.Clear();
        _costSoFar.Clear();
        var frontier = new SimplePriorityQueue<Node>();

        frontier.Enqueue(_currentNode, 0);
        _cameFrom[_currentNode] = _currentNode;
        _costSoFar[_currentNode] = 0;

        while (frontier.Count > 0)
        {
            var currentNode = frontier.Dequeue();

            if (currentNode == targetNode)
                break;

            foreach (var nextNode in currentNode.Neighbours)
            {
                var newCost = _costSoFar[currentNode] + nextNode.Cost;
                if (!_costSoFar.TryGetValue(nextNode, out int nextNodePrevCost)
                    || newCost < nextNodePrevCost)
                {
                    _costSoFar[nextNode] = newCost;
                    var priority = newCost + heuristic(nextNode, targetNode);
                    frontier.Enqueue(nextNode, priority);
                    _cameFrom[nextNode] = currentNode;
                }
            }
        }

        _currentPath.Clear();
        Node currentPathNode = targetNode;
        Node previousNode = null;
        while (_cameFrom.TryGetValue(currentPathNode, out previousNode))
        {
            _currentPath[previousNode] = currentPathNode;
            currentPathNode = previousNode;

            if (currentPathNode == _currentNode)
            {
                Logger.Log($"[AI] Start node found : {previousNode} => {_currentNode}");
                break;
            }
        }
#if UNITY_EDITOR
        editorDisplayPath(); 
#endif
    }

    public Vector3 GetNextPosition()
    {
        if(_currentNode == null)
        {
            Logger.LogError("No current node set, can't get next position");
            return transform.position;
        }
        if(!_currentPath.TryGetValue(_currentNode, out Node nextNode))
        {
            Logger.LogError("Current node not in current path, can't get next position");
            return transform.position;
        }
        if(nextNode == null)
        {
            Logger.LogError("Empty next node, can't get next position");
            return transform.position;
        }

        _currentNode = nextNode;
        return _currentNode.transform.position;
    }

    public Vector3 PeekNextPosition()
    {
        if (_currentNode == null)
        {
            Logger.LogError("No current node set, can't get next position");
            return transform.position;
        }
        if (!_currentPath.TryGetValue(_currentNode, out Node nextNode))
        {
            Logger.LogError("Current node not in current path, can't get next position");
            return _currentNode.transform.position;
        }
        if (nextNode == null)
        {
            Logger.LogError("Empty next node, can't peek next position");
            return _currentNode.transform.position;
        }

        return nextNode.transform.position;
    }

    private float heuristic(Node nodeA, Node nodeB)
    {
        // Using a Manhattan distance heuristic here because the agent
        // can move in 4 directions in the grid
        Vector3 positionA = nodeA.transform.position;
        Vector3 positionB = nodeB.transform.position;
        return Mathf.Abs(positionA.x - positionB.x) + Mathf.Abs(positionA.y - positionB.y);
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

    private void editorDisplayPath()
    {
        _gridReference.Reference.ResetNodesSelection();

        var targetNode = _currentTarget.CurrentNode;
        Node currentDisplayNode = _currentNode;
        Node nextNode = null;
        while (_currentPath.TryGetValue(currentDisplayNode, out nextNode))
        {
            currentDisplayNode = nextNode;
            currentDisplayNode.SelectNode(true);

            if (currentDisplayNode == _currentNode)
            {
                Logger.Log($"[AI] End node found : {currentDisplayNode} => {targetNode}");
                currentDisplayNode.SelectNode(true);
                break;
            }
        }
    }
#endif
}
