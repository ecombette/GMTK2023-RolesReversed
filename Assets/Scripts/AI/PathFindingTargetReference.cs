using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "AITargetReference", menuName = "AI/Target Reference")]
public class PathFindingTargetReference : ScriptableObject
{
    [SerializeField]
    private PathFindingTarget _reference;

    public UnityAction OnTargetMove;

    public PathFindingTarget Reference => _reference;
    public Transform TargetTransform => _reference == null ? null : _reference.transform;
    public Vector3 TargetPosition => _reference == null ? Vector3.zero : _reference.transform.position;
    public Quaternion TargetRotation => _reference == null ? Quaternion.identity : _reference.transform.rotation;

    public void Init(PathFindingTarget reference)
    {
        _reference = reference;
    }

    public void ResetTarget()
    {
        _reference = null;
        OnTargetMove = null;
    }
}
