using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GD.MinMaxSlider;

public class GoldPileVisual : MonoBehaviour
{
    [SerializeField] GoldPile _goldPile;
    [Space]
    [SerializeField, MinMaxSlider(0f,5f)] Vector2 _delayAtStart;
    [SerializeField] AnimationCurve _emmisiveIntensityAnimationCurve;
    [SerializeField] float _speed = 0.5f;
    [SerializeField] List<Renderer> _renderers;
    List<Material> _materials;

    [Header("FX")]
    [SerializeField] GameObject _goldParticle;
    [SerializeField] ParticleSystem _goldParticlePickedUP;

    public void OnEnable()
    {
        _materials = new List<Material>();
        for (int i = 0; i < _renderers.Count; i++)
        {
            _materials.Add(_renderers[i].material);
        }

        _goldPile.OnPickedUp += pickUp;

        StartCoroutine(animateEmmisive());

        IEnumerator animateEmmisive()
        {
            yield return new WaitForSeconds(Random.Range(_delayAtStart.x, _delayAtStart.y));

            float t = 0f;

            while (true)
            {
                t += Time.deltaTime * _speed;

                for (int i = 0; i < _materials.Count; i++)
                {
                    _materials[i].SetVector("_EmissionColor", _materials[i].color * _emmisiveIntensityAnimationCurve.Evaluate(t));

                    _renderers[i].material = _materials[i];
                }

                if (t > 1)
                    t = 0f;

                yield return null;
            }
        }
    }

    private void pickUp()
    {
        _goldPile.OnPickedUp -= pickUp;

        _goldParticle.SetActive(false);
        _goldParticlePickedUP.Play();

        for (int i = 0; i < _renderers.Count; i++)
        {
            _renderers[i].gameObject.SetActive(false);
        }
    }

    public void OnDisable()
    {
        StopAllCoroutines();
    }
}
