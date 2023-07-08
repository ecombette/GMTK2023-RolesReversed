using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEditor;
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
    }

#if UNITY_EDITOR
    [ContextMenu("Refresh Child Nodes List")]
    public void EditorRefreshChildNodesList()
    {
        if (_nodes == null)
            _nodes = new List<Node>();
        else
            _nodes.Clear();

        _nodes.AddRange(GetComponentsInChildren<Node>());
        EditorUtility.SetDirty(this);
    }

    [ContextMenu("Refresh Nodes Neighbours")]
    public void EditorRefreshNodesNeighbourd()
    {
        foreach (var node in _nodes)
            node.EditorRefreshNeighbours();
    }
#endif
}
