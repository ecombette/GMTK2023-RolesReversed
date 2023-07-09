using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class LevelManager : MonoBehaviour
{
    [SerializeField]
    private GoldPile[] _goldPiles;
    public UnityEvent OnAllPilesPickedUp;
    [SerializeField] ChestController chestController;

    private void OnEnable()
    {
        foreach (var goldPile in _goldPiles)
        {
            goldPile.Init();
            goldPile.OnPickedUp += onGoldPilePickedUp;
        }
    }

    private void Start()
    {
        GameManager.Instance.SubscribeToGameOver(GameOver);
    }

    private void OnDestroy()
    {
        GameManager.Instance.UnsubscribeFromGameOver(GameOver);
    }

    private void onGoldPilePickedUp()
    {
        foreach (var goldPile in _goldPiles)
        {
            if (goldPile && !goldPile.IsPickedUp)
                return;
        }

        Logger.Log("All gold piles picked up, triggering end of level");
        OnAllPilesPickedUp?.Invoke();
    }

    [ContextMenu("Door Reached")]
    public void OnDoorReached()
    {
        chestController.enabled = false;

        StartCoroutine(delay());

        IEnumerator delay()
        {
            yield return new WaitForSeconds(1f);
            GameManager.Instance.LoadNextLevel();
        }
    }

    public void GameOver()
    {
        chestController.KillChest();
    }
}
