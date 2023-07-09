using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "GridReference", menuName = "AI/Grid Reference")]
public class GridReference : ScriptableObject
{
    [SerializeField]
    private LevelGrid _reference;

    private UnityAction _onInitialized;

    public bool IsInitialized => _reference != null;
    public LevelGrid Reference => _reference;
    public ReadOnlyCollection<Node> Nodes => _reference == null ? null : _reference.Nodes;

    public void Init(LevelGrid reference)
    {
        _reference = reference;
        _onInitialized?.Invoke();
    }

    public void SubscribeToInitialization(UnityAction onInitialized)
    {
        _onInitialized += onInitialized;
    }

    public void UnsubscribeFromInitialization(UnityAction onInitialized)
    {
        _onInitialized -= onInitialized;
    }
}
