using UnityEngine;

[CreateAssetMenu(fileName = "AITargetReference", menuName = "AI/Target Reference")]
public class PathFindingTargetReference : ScriptableObject
{
    [SerializeField]
    private PathFindingTarget _reference;

    public PathFindingTarget Reference => _reference;
    public Node CurrentNode => _reference == null ? null : _reference.CurrentNode;
    public Transform TargetTransform => _reference == null ? null : _reference.transform;
    public Vector3 TargetPosition => _reference == null ? Vector3.zero : _reference.transform.position;
    public Quaternion TargetRotation => _reference == null ? Quaternion.identity : _reference.transform.rotation;

    public void Init(PathFindingTarget reference)
    {
        _reference = reference;
    }

    public void ResetTarget()
    {
        if(_reference)
            _reference.ResetListeners();

        _reference = null;
    }
}
