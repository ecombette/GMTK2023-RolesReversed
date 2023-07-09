using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.Events;

public class Node : MonoBehaviour
{
    [SerializeField]
    private GridReference _gridReference;
    [SerializeField]
    private bool _isWalkable = true;
    [SerializeField]
    private int _nodeCost = 1;
    [SerializeField]
    private List<Node> _neighbours;
    [SerializeField]
    private Node[] _directionalNeighbourhood = new Node[DirectionUtility.DirectionCount];

    [SerializeField] private float _searchRadius = 1.1f;

    [SerializeField]
    private UnityEvent _onNodeSelected, _onNodeUnselected;

    public int Cost => _nodeCost;
    public ReadOnlyCollection<Node> Neighbours => _neighbours.AsReadOnly();

    public bool IsWalkable => _isWalkable;
    public bool HasNeighbour(Direction direction) => _directionalNeighbourhood[(int)direction] != null;
    public Node GetNeighbour(Direction direction) => _directionalNeighbourhood[(int)direction];

    public void SelectNode(bool selected)
    {
        if (selected)
        {
            Logger.Log($"Selecting node {gameObject.name}", gameObject);
            _onNodeSelected?.Invoke();
        }
        else
            _onNodeUnselected?.Invoke();
    }

    public void SetWalkable(bool walkable)
    {
        _isWalkable = walkable;
    }

#if UNITY_EDITOR
    [ContextMenu("Refresh Neighbours")]
    public void EditorRefreshNeighbours()
    {
        editorClearDirectionalNeighbourhood();
        if (_neighbours == null)
            _neighbours = new List<Node>();
        else
            _neighbours.Clear();

        var nodePosition = transform.position;
        float sqrSearchRadius = _searchRadius * _searchRadius;
        var gridNodes = _gridReference.Nodes;
        foreach(var node in gridNodes)
        {
            if(node == this)
                continue;

            var nodesDelta = node.transform.position - nodePosition;
            if(Vector3.SqrMagnitude(nodesDelta) <= sqrSearchRadius)
            {
                _neighbours.Add(node);
                var horizontalDot = Vector3.Dot(nodesDelta, Vector3.right);
                var verticalDot = Vector3.Dot(nodesDelta, Vector3.forward);
                if(Mathf.Abs(verticalDot) > .9f)
                {
                    if(verticalDot > 0f)
                        _directionalNeighbourhood[(int)Direction.UP] = node;
                    else
                        _directionalNeighbourhood[(int)Direction.DOWN] = node;
                }
                else if(Mathf.Abs(horizontalDot) > .9f)
                {
                    if (horizontalDot > 0f)
                        _directionalNeighbourhood[(int)Direction.RIGHT] = node;
                    else
                        _directionalNeighbourhood[(int)Direction.LEFT] = node;
                }
            }
        }

        UnityEditor.EditorUtility.SetDirty(this);
    }

    private void editorClearDirectionalNeighbourhood()
    {
        for(int i = 0; i < DirectionUtility.DirectionCount; i++)
            _directionalNeighbourhood[i] = null;
    }
#endif
}
