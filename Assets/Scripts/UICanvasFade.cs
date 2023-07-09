using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICanvasFade : MonoBehaviour
{
    Coroutine _fadeCoroutine = null;
    [SerializeField] Material _fadeMaterial;
    [Header("Audio")]
    [SerializeField] AudioSource _source;
    [SerializeField] AudioClip _fadeIn;
    [SerializeField] AudioClip _fadeOut;

    public void OnEnable()
    {
        FadeIn();
    }

    [ContextMenu("Fade In")]
    public void FadeIn()
    {
        if(_fadeCoroutine != null)
        {
            StopCoroutine(_fadeCoroutine);
            _source.Stop();
        }

        _fadeCoroutine = StartCoroutine(fade(0f, 1f, 2f));

        _source.clip = _fadeIn;
        _source.Play();
    }

    [ContextMenu("Fade Out")]
    public void FadeOut()
    {
        if (_fadeCoroutine != null)
        {
            StopCoroutine(_fadeCoroutine);
            _source.Stop(); 
        }

        _fadeCoroutine = StartCoroutine(fade(1f, 0f, 3.5f));

        _source.clip = _fadeOut;
        _source.Play();
    }

    IEnumerator fade(float alphaStart, float alphaEnd, float speed)
    {
        float t = 0f;

        while(t < 1f)
        {
            t += Time.deltaTime * speed;
            _fadeMaterial.SetFloat("_Progress", Mathf.Lerp(alphaStart, alphaEnd, t));
            yield return null;
        }
    }
}
