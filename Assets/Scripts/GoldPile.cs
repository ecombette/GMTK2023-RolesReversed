using UnityEngine;
using UnityEngine.Events;

public class GoldPile : MonoBehaviour
{
    [SerializeField]
    private bool _isPickedUp = false;

    public bool IsPickedUp => _isPickedUp;
    public UnityAction OnPickedUp;
    [SerializeField] Collider _collider;

    private void OnTriggerEnter(Collider other)
    {
        if(!_isPickedUp && other.CompareTag("Player"))
            PickUp();
    }

    public void Init()
    {
        _isPickedUp = false;
    }

    [ContextMenu("Pick Up")]
    public void PickUp()
    {
        if(_isPickedUp)
        {
            Logger.LogWarning("Trying to pick up already picked up gold pile");
            return;
        }

        _isPickedUp = true;
        OnPickedUp?.Invoke();
        _collider.enabled = false;
    }
}
