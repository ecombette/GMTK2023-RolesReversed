using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class DoorManager : MonoBehaviour
{
    [SerializeField] LevelManager _levelManager;
    [SerializeField] UnityEvent _onDoorUnlocked;

    [Header("Lock")]
    [SerializeField] GameObject _lock;
    [SerializeField] ParticleSystem _lockParticle;
    [SerializeField] Animator _doorAnimator;
    static int HASH_DOOROPEN = Animator.StringToHash("Open");

    [Header("Audio")]
    [SerializeField] AudioSource _source;
    [SerializeField] AudioClip _doorOpenClip;

    private void OnEnable()
    {
        _levelManager.OnAllPilesPickedUp.AddListener(unlockDoor);
    }

    private void OnDisable()
    {
        _levelManager.OnAllPilesPickedUp.RemoveListener(unlockDoor);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
            _levelManager.OnDoorReached();
    }

    [ContextMenu("Unlock Door")]
    private void unlockDoor()
    {
        StartCoroutine(delayUnlockDoor());

        IEnumerator delayUnlockDoor()
        {
            yield return new WaitForSeconds(0.5f);
            Logger.Log("Unlocking door");
            _lock.SetActive(false);
            _lockParticle.Play();
            _doorAnimator.SetTrigger(HASH_DOOROPEN);

            if(_source && _doorOpenClip)
            {
                _source.clip = _doorOpenClip;
                _source.Play();
            }

            _onDoorUnlocked?.Invoke();
        }
    }
}
