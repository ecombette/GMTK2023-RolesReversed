using System.Collections.ObjectModel;
using UnityEngine;

[CreateAssetMenu(fileName = "GridReference", menuName = "AI/Grid Reference")]
public class GridReference : ScriptableObject
{
    [SerializeField]
    private LevelGrid _reference;
    public LevelGrid Reference => _reference;
    public ReadOnlyCollection<Node> Nodes => _reference == null ? null : _reference.Nodes;

    public void Init(LevelGrid reference)
    {
        _reference = reference;
    }
}
