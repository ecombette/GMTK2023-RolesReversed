using UnityEngine;
using UnityEngine.Events;

public class LevelManager : MonoBehaviour
{
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
        var gameManager = GameManager.Instance;
        if (gameManager == null)
        {
            Logger.LogError("No game manager found, won't load next level");
            return;
        }

        gameManager.LoadNextLevel();
    }
}
