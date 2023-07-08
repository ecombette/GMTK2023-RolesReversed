using UnityEngine;

public class PathFindingTarget : MonoBehaviour
{
    [SerializeField]
    private PathFindingTargetReference _targetReference;

    private void Awake()
    {
        if (_targetReference == null)
        {
            Logger.LogWarning("No level grid reference in script, won't set reference");
            return;
        }

        _targetReference.Init(this);
    }

    public void SetTargetMove()
    {
        
    }
}
