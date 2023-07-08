using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICanvasFade : MonoBehaviour
{
    Coroutine _fadeCoroutine = null;
    [SerializeField] Material _fadeMaterial;

    public void OnEnable()
    {
        FadeIn();
    }

    [ContextMenu("Fade In")]
    public void FadeIn()
    {
        if(_fadeCoroutine != null)
            StopCoroutine(_fadeCoroutine);

        _fadeCoroutine = StartCoroutine(fade(0f, 1f, 2f));
    }

    [ContextMenu("Fade Out")]
    public void FadeOut()
    {
        if (_fadeCoroutine != null)
            StopCoroutine(_fadeCoroutine);

        _fadeCoroutine = StartCoroutine(fade(1f, 0f, 3.5f));
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
