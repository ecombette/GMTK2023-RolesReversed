using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Node : MonoBehaviour
{
    [SerializeField]
    private GridReference _gridReference;
    [SerializeField]
    private List<Node> _neighbours;

    [SerializeField]
    private MeshRenderer _meshRenderer;

#if UNITY_EDITOR
    [ContextMenu("Refresh Neighbours")]
    public void EditorRefreshNeighbours()
    {
        if (_neighbours == null)
            _neighbours = new List<Node>();
        else
            _neighbours.Clear();

        var bounds = _meshRenderer.bounds;
        var boundsCenter = bounds.center;
        var boundsSize = bounds.size;
        float searchRadius = Mathf.Max(boundsSize.x, boundsSize.z) + .1f;
        float sqrSearchRadius = searchRadius * searchRadius;

        var gridNodes = _gridReference.Nodes;
        foreach(var node in gridNodes)
        {
            if(node == this)
                continue;

            if(Vector3.SqrMagnitude(boundsCenter - node.transform.position) <= sqrSearchRadius)
                _neighbours.Add(node);
        }

        EditorUtility.SetDirty(this);
    }
#endif
}
