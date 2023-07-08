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

    //[SerializeField]
    //private MeshRenderer _meshRenderer;

    [SerializeField] private float _searchRadius = 1.1f;

    [SerializeField]
    private UnityEvent _onNodeSelected, _onNodeUnselected;

    public int Cost => _nodeCost;
    public ReadOnlyCollection<Node> Neighbours => _neighbours.AsReadOnly();

    public void SelectNode(bool selected)
    {
        if (selected)
            _onNodeSelected?.Invoke();
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

        //var bounds = _meshRenderer.bounds;
        var boundsCenter = transform.position;
        //var boundsSize = _radius;
        //float searchRadius = Mathf.Max(boundsSize.x, boundsSize.z) + .1f;
        float sqrSearchRadius = _searchRadius * _searchRadius;

        var gridNodes = _gridReference.Nodes;
        foreach(var node in gridNodes)
        {
            if(node == this)
                continue;

            if(Vector3.SqrMagnitude(boundsCenter - node.transform.position) <= sqrSearchRadius)
                _neighbours.Add(node);
        }

        UnityEditor.EditorUtility.SetDirty(this);
    }
#endif
}
