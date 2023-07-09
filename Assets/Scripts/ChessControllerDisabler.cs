using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessControllerDisabler : MonoBehaviour
{
    [SerializeField] ChestController _chestController;
    bool _canBeToggled = true;

    private void Start()
    {
        GameManager.Instance.InLevelLoadingMenu += toggleChestController;
        GameManager.Instance.SubscribeToGameOver(cantBeToggle);
        GameManager.Instance.SubscribeToLevelCompleted(cantBeToggle);
    }

    private void toggleChestController(bool state)
    {
        if (_canBeToggled)
            _chestController.enabled = !state;
    }

    private void OnDestroy()
    {
        GameManager.Instance.InLevelLoadingMenu += toggleChestController;

        GameManager.Instance.UnsubscribeFromGameOver(cantBeToggle);
        GameManager.Instance.UnsubscribeFromLevelCompleted(cantBeToggle);
    }

    private void cantBeToggle()
    {
        _canBeToggled = false; //To avoid player spaming menu to re toggle movement while dead or completing the level
    }
}
