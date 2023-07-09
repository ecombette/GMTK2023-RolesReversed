using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyTutorial : MonoBehaviour
{
    [SerializeField] SpriteRenderer _renderer;
    bool _keyPressed = false;

    public void Update()
    {
        float horizontalAxis = Input.GetAxis("Horizontal");

        if (horizontalAxis > 0f && !_keyPressed)
        {
            StartCoroutine(lerpColor());
            _keyPressed = true;
        }
    }

    IEnumerator lerpColor()
    {
        float t = 0f;

        while(t < 1f)
        {
            t += Time.deltaTime;

            Color color = _renderer.color;
            color.a = 1 - t;

            _renderer.color = color;

            yield return null;
        }

        gameObject.SetActive(false);
    }
}
