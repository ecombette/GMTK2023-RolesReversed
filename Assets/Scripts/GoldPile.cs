using UnityEngine;
using UnityEngine.Events;

public class GoldPile : MonoBehaviour
{
    [SerializeField]
    private bool _isPickedUp = false;

    public bool IsPickedUp => _isPickedUp;
    public UnityAction OnPickedUp;

    public void Init()
    {
        _isPickedUp = false;
    }

    public void PickUp()
    {
        if(_isPickedUp)
        {
            Logger.LogWarning("Trying to pick up already picked up gold pile");
            return;
        }

        _isPickedUp = true;
        OnPickedUp?.Invoke();
    }
}
