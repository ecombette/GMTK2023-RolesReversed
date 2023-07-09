using System.Collections;
using UnityEngine;

public class DoorManager : MonoBehaviour
{
    [SerializeField] LevelManager _levelManager;
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
        }
    }

    [ContextMenu("Load Next Level")]
    public void LoadNextLevel()
    {
        if (GameManager.Instance)
            GameManager.Instance.LoadNextLevel();
    }


    private void OnDisable()
    {
        _levelManager.OnAllPilesPickedUp.RemoveListener(unlockDoor);
    }
}
