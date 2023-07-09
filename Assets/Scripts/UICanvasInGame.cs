using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICanvasInGame : MonoBehaviour
{
    [SerializeField] bool _forceEnable = false;

    [SerializeField] Animator _animator;
    static int HASH_ENABLE = Animator.StringToHash("Enable");

    bool _levelLoadingMenuState;
    [SerializeField] GameObject _leveLoadingMenuContent;
    [SerializeField] Sprite _buttonOn;
    [SerializeField] Sprite _buttonOff;
    [SerializeField] Image _buttonImage;
    [SerializeField] List<Button> _buttons;

    private void OnEnable()
    {
        if(_forceEnable)
            EnableInterface();
    }

    public void EnableInterface()
    {
        _animator.SetBool(HASH_ENABLE, true);
    }

    public void DisableInterface()
    {
        _animator.SetBool(HASH_ENABLE, false);
    }

    public void ToggleLevelLoadingMenu()
    {
        _levelLoadingMenuState = !_levelLoadingMenuState;
        _leveLoadingMenuContent.SetActive(_levelLoadingMenuState);

        _buttonImage.sprite = _levelLoadingMenuState ? _buttonOff : _buttonOn;

        GameManager.Instance.IsInLevelLoadingMenu(_levelLoadingMenuState);
    }
}
