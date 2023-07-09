using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstLoadingScreenDelay : MonoBehaviour
{
    [SerializeField] GameManager _gameManager;
    [SerializeField] float _delay;

    public void OnEnable()
    {
        StartCoroutine(delay());

        IEnumerator delay()
        {
            yield return new WaitForSeconds(_delay);
            _gameManager.LoadNextLevel();
        }
    }
}
