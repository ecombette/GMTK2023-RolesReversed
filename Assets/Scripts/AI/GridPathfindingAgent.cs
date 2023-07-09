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

    public bool NextPositionIsTarget => 
        _currentTarget != null && _currentTarget.CurrentNode != null
        && _currentPath.TryGetValue(_currentNode, out Node nextNode)
        && nextNode == _currentTarget.CurrentNode;

    public bool IsTargetNode(Node node)
        => node != null && _currentTarget != null && _currentTarget.CurrentNode == node;

    private void Awake()
    {
        RefreshCurrentNode();
    }

    public void FindPath(PathFindingTargetReference targetReference)
    {
        if(targetReference == null || targetReference.Reference == null)
        {
            Logger.LogError($"[AI] Agent {gameObject.name} trying to find path to empty target, aborting");
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
                Logger.Log($"[AI] Agent {gameObject.name} start node found : {_currentPath[previousNode]} => {_currentNode}");
                break;
            }
        }
#if UNITY_EDITOR
        displayPath(); 
#endif
    }

    public Node GetNextNode()
    {
        if (_currentNode == null)
        {
            Logger.LogError($"[AI] Agent {gameObject.name} no current node set, can't get next node");
            return null;
        }
        if (!_currentPath.TryGetValue(_currentNode, out Node nextNode))
        {
            Logger.LogError($"[AI] Agent {gameObject.name} current node not in current path, can't get next node");
            return null;
        }
        if (nextNode == null)
        {
            Logger.LogError($"[AI] Agent {gameObject.name} empty next node, can't get next node");
            return null;
        }

        _currentNode = nextNode;
        return _currentNode;
    }

    public Vector3 GetNextPosition()
    {
        GetNextNode();
        return _currentNode ? _currentNode.transform.position : transform.position;
    }

    public Node PeekNextNode()
    {
        if (_currentNode == null)
        {
            Logger.LogError($"[AI] Agent {gameObject.name} no current node set, can't peek next node");
            return null;
        }
        if (!_currentPath.TryGetValue(_currentNode, out Node nextNode))
        {
            Logger.LogError($"[AI] Agent {gameObject.name} current node not in current path, can't peek next node");
            return null;
        }

        return nextNode;
    }

    public Vector3 PeekNextPosition()
    {
        if (_currentNode == null)
        {
            Logger.LogError($"[AI] Agent {gameObject.name} no current node set, can't get next position");
            return transform.position;
        }
        if (!_currentPath.TryGetValue(_currentNode, out Node nextNode))
        {
            Logger.LogError($"[AI] Agent {gameObject.name} current node not in current path, can't get next position");
            return _currentNode.transform.position;
        }
        if (nextNode == null)
        {
            Logger.LogError($"[AI] Agent {gameObject.name} empty next node, can't peek next position");
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

    [ContextMenu("Refresh Current Node")]
    public void RefreshCurrentNode()
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
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }

    private void displayPath()
    {
        _gridReference.Reference.ResetNodesSelection();

        var targetNode = _currentTarget.CurrentNode;
        Node currentDisplayNode = _currentNode;
        Node nextNode;
        while (_currentPath.TryGetValue(currentDisplayNode, out nextNode))
        {
            currentDisplayNode = nextNode;
            currentDisplayNode.SelectNode(true);

            if (currentDisplayNode == _currentNode)
            {
                Logger.Log($"[AI] Agent {gameObject.name} end node found : {currentDisplayNode} => {targetNode}");
                currentDisplayNode.SelectNode(true);
                break;
            }
        }
    }
}
