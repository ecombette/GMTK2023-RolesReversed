using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestDeath : MonoBehaviour
{

    [SerializeField] Transform _chest;
    [SerializeField] Transform _chestDead;
    [SerializeField] GameObject _groundCollider;

    [Header("Audio")]
    [SerializeField] AudioSource _source;
    [SerializeField] AudioClip _dieSFX;


    [ContextMenu("Kill Chest")]
    public void KillChest()
    {
        StartCoroutine(delay());

        IEnumerator delay()
        {
            yield return new WaitForSeconds(0.5f);

            _groundCollider.SetActive(true);
            _chest.gameObject.SetActive(false);
            _chestDead.gameObject.SetActive(true);
            playSound(_dieSFX);
            this.enabled = false;
        }
    }

    private void playSound(AudioClip clip)
    {
        if (_source && clip)
        {
            _source.clip = clip;
            _source.Play();
        }
    }
}
