using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.Events;

public class Node : MonoBehaviour
{
    [SerializeField]
    private GridReference _gridReference;
    [SerializeField]
    private int _nodeCost = 1;
    [SerializeField]
    private List<Node> _neighbours;

    [SerializeField] private float _searchRadius = 1.1f;

    [SerializeField]
    private UnityEvent _onNodeSelected, _onNodeUnselected;

    public int Cost => _nodeCost;
    public ReadOnlyCollection<Node> Neighbours => _neighbours.AsReadOnly();

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

#if UNITY_EDITOR
    [ContextMenu("Refresh Neighbours")]
    public void EditorRefreshNeighbours()
    {
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

            if(Vector3.SqrMagnitude(nodePosition - node.transform.position) <= sqrSearchRadius)
                _neighbours.Add(node);
        }

        UnityEditor.EditorUtility.SetDirty(this);
    }
#endif
}
