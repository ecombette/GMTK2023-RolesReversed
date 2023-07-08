using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICanvasInGame : MonoBehaviour
{
    [SerializeField] Animator _animator;
    static int HASH_ENABLE = Animator.StringToHash("Enable");

    public void EnableInterface()
    {
        _animator.SetBool(HASH_ENABLE, true);
    }

    public void DisableInterface()
    {
        _animator.SetBool(HASH_ENABLE, false);
    }
}
