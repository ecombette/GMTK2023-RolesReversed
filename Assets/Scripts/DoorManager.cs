using UnityEngine;

public class DoorManager : MonoBehaviour
{
    [Header("Lock")]
    [SerializeField] GameObject _lock;
    [SerializeField] ParticleSystem _lockParticle;
    [SerializeField] Animator _doorAnimator;
    static int HASH_DOOROPEN = Animator.StringToHash("Open");

    [ContextMenu("Unlock Door")]
    public void UnlockDoor()
    {
        Logger.Log("Unlocking door");
        _lock.SetActive(false);
        _lockParticle.Play(); 
        _doorAnimator.SetTrigger(HASH_DOOROPEN);
    }
}
