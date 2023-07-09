using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroCameraSequencer : MonoBehaviour
{
    [SerializeField] float _duration = 5f;
    CameraFollowPlayer _cameraFollowPlayer;
    [SerializeField] UICanvasInGame _canvasInGame;

    [SerializeField] CanvasFadeIn _canvasTutorial;

    private void OnEnable()
    {
        StartSequence();
    }

    public void StartSequence()
    {
        _cameraFollowPlayer = Camera.main.GetComponent<CameraFollowPlayer>();

        StartCoroutine(durationCoroutine());

        IEnumerator durationCoroutine()
        {
            yield return new WaitForSeconds(_duration);

            _canvasInGame.EnableInterface();

            if (_cameraFollowPlayer)
            {
                _cameraFollowPlayer.ZoomToPlayer();
                _cameraFollowPlayer.FollowPlayer();
            }

            if (_canvasTutorial)
                _canvasTutorial.FadeIn();
        }
    }
}
