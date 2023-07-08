using System.Collections.Generic;
using UnityEngine;
using Priority_Queue;

public class GridPathfinding : MonoBehaviour
{
    [SerializeField]
    private GridReference _gridReference;
    [SerializeField]
    private Node _currentNode;

    private PathFindingTargetReference _currentTarget;
    private Dictionary<Node, Node> _cameFrom = new Dictionary<Node, Node>();
    private Dictionary<Node, int> _costSoFar = new Dictionary<Node, int>();

    public void FindPath(PathFindingTargetReference targetReference)
    {
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

        var nodeStack = new Stack<Node>();
        var currentDisplayNode = targetNode;
        nodeStack.Push(targetNode);
        while (_cameFrom.TryGetValue(currentDisplayNode, out Node previousNode))
        {
            Logger.Log($"[AI] Backtracking : {previousNode} => {currentDisplayNode}");
            currentDisplayNode.SelectNode(true);
            nodeStack.Push(previousNode);
            currentDisplayNode = previousNode;

            if(currentDisplayNode == _currentNode)
            {
                Logger.Log($"[AI] Start node found : {previousNode} => {_currentNode}");
                break;
            }
        }
    }

    public Vector3 GetNextPosition()
    {
        //TODO 0:)
        return Vector3.zero;
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
#endif
}
