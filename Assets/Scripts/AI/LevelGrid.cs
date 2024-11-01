using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class LevelGrid : MonoBehaviour
{
    [SerializeField]
    private GridReference _gridReference;
    [SerializeField]
    private List<Node> _nodes;

    public ReadOnlyCollection<Node> Nodes => _nodes.AsReadOnly();

    private void Awake()
    {
        if (_gridReference == null)
        {
            Logger.LogWarning("No level grid reference in script, won't set reference");
            return;
        }

        _gridReference.Init(this);
        ResetNodesSelection();
    }

    [ContextMenu("Reset Nodes Selection")]
    public void ResetNodesSelection()
    {
        foreach (var node in _nodes)
            node.SelectNode(false);
    }


#if UNITY_EDITOR
    [ContextMenu("Init Grid Reference")]
    public void InitGridReference()
    {
        _gridReference.Init(this);
    }

    [ContextMenu("Refresh Child Nodes List")]
    public void EditorRefreshChildNodesList()
    {
        if (_nodes == null)
            _nodes = new List<Node>();
        else
            _nodes.Clear();

        _nodes.AddRange(GetComponentsInChildren<Node>());
        UnityEditor.EditorUtility.SetDirty(this);
    }

    [ContextMenu("Refresh Nodes Neighbours")]
    public void EditorRefreshNodesNeighbourd()
    {
        foreach (var node in _nodes)
            node.EditorRefreshNeighbours();
    }
#endif
}
