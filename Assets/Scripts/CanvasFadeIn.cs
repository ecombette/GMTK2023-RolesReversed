using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CanvasFadeIn : MonoBehaviour
{
    [SerializeField] List<Image> _images;
    [SerializeField] List<TextMeshProUGUI> _text;

    [ContextMenu("Fade In")]
    public void FadeIn()
    {
        StartCoroutine(fadeIn());

        IEnumerator fadeIn()
        {
            float t = 0f;

            while(t < 1f)
            {
                t += Time.deltaTime;

                for (int i = 0; i < _images.Count; i++)
                {
                    Color color = _images[i].color;
                    color.a = t;

                    _images[i].color = color;
                }

                for (int i = 0; i < _text.Count; i++)
                {
                    Color color = _text[i].color;
                    color.a = t;

                    _text[i].color = color;
                }

                yield return null;
            }
        }
    }
}
