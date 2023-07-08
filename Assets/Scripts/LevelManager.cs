using UnityEngine;
using UnityEngine.Events;

public class LevelManager : MonoBehaviour
{
    [SerializeField]
    private GameManager _gameManager;
    [SerializeField]
    private GoldPile[] _goldPiles;
    [SerializeField]
    private UnityEvent _onAllPilesPickedUp;

    private void OnEnable()
    {
        foreach(var goldPile in _goldPiles)
        {
            goldPile.Init();
            goldPile.OnPickedUp += onGoldPilePickedUp;
        }
    }

    private void onGoldPilePickedUp()
    {
        foreach(var goldPile in _goldPiles)
        {
            if (goldPile && !goldPile.IsPickedUp)
                return;
        }

        Logger.Log("All gold piles picked up, triggering end of level");
        _onAllPilesPickedUp?.Invoke();
    }

    public void LoadNextLevel()
    {
        if (_gameManager == null)
        {
            Logger.LogError("No game manager referenced, won't load next level");
            return;
        }

        _gameManager.LoadNextLevel();
    }
}
