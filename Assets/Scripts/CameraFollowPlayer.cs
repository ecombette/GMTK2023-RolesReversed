using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    [SerializeField] Camera _camera;
    [SerializeField] float _orthoSizeToPlayer = 2.25f;

    [SerializeField] Transform _player;
    [SerializeField] Vector3 _offsetToPlayer = new Vector3(2.192f, 3.13f, -1.76f);
    [SerializeField] float _lerpSpeed = 5f;
    float _defaultY;
    [SerializeField] AnimationCurve _zoomAnimationCurve;

    private void OnEnable()
    {
        _defaultY = _player.transform.position.y + _offsetToPlayer.y;
        //FollowPlayer();
    }

    public void FollowPlayer()
    {
        StartCoroutine(followPlayer());
    }

    IEnumerator followPlayer()
    {
        while (true)
        {
            Vector3 targetPos = _player.position + _offsetToPlayer;
            targetPos = new Vector3(targetPos.x, _defaultY, targetPos.z);

            transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * _lerpSpeed);
            yield return null;
        }
    }

    public void ZoomToPlayer()
    {
        StartCoroutine(zoomToPlayer());
    }

    IEnumerator zoomToPlayer()
    {
        float startOrtho = _camera.orthographicSize;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * _lerpSpeed;

            _camera.orthographicSize = Mathf.Lerp(startOrtho, _orthoSizeToPlayer, _zoomAnimationCurve.Evaluate(t));
            yield return null;
        }
    }

    public void StopFollowingPlayer()
    {
        StopAllCoroutines();
    }
}
