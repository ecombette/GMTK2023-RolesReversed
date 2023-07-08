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
