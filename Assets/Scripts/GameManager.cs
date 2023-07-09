using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField][Range(0f, 10f)]
    private float _delayBeforeLoadNext = 1f;
    [SerializeField][Range(0f, 10f)]
    private float _delayBeforeReloading = 1f;

    [Header("Levels")]
    [SerializeField] private LevelList _levelList;
    [SerializeField] UICanvasFade _canvasFade;
    [SerializeField] private UnityEvent<int> _onStartLoadingScene, _onSceneLoaded;

    private UnityAction _onLevelCompleted, _onGameOver;
    public UnityAction<bool> InLevelLoadingMenu;

    public void Awake()
    {
        Instance = this;

#if UNITY_EDITOR
        _levelList.EditorTrySetCurrentLevel(SceneManager.GetActiveScene().name);
#endif
    }

    #region Game Events
    [ContextMenu("Game Over")]
    public void GameOver()
    {
        _onGameOver?.Invoke();
        StartCoroutine(delayedReloadCoroutine());

        IEnumerator delayedReloadCoroutine()
        {
            if (_delayBeforeReloading > 0f)
                yield return new WaitForSeconds(_delayBeforeReloading);
            else
                yield return null;

            LoadCurrentLevel();
        }
    }

    public void SubscribeToGameOver(UnityAction onGameOver)
    {
        _onGameOver += onGameOver;
    }

    public void UnsubscribeFromGameOver(UnityAction onGameOver)
    {
        _onGameOver -= onGameOver;
    }

    [ContextMenu("Level Completed")]
    public void LevelCompleted()
    {
        _onLevelCompleted?.Invoke();
        StartCoroutine(delayedLoadNextCoroutine());

        IEnumerator delayedLoadNextCoroutine()
        {
            if (_delayBeforeReloading > 0f)
                yield return new WaitForSeconds(_delayBeforeLoadNext);
            else
                yield return null;

            LoadNextLevel();
        }
    }

    public void SubscribeToLevelCompleted(UnityAction onLevelCompleted)
    {
        _onLevelCompleted += onLevelCompleted;
    }

    public void UnsubscribeFromLevelCompleted(UnityAction onLevelCompleted)
    {
        _onLevelCompleted -= onLevelCompleted;
    } 
    #endregion

    #region Level Loading
    [ContextMenu("Load First Level")]
    public void LoadFirstLevel()
    {
        LoadLevel(0);
    }

    [ContextMenu("Load Current Level")]
    public void LoadCurrentLevel()
    {
        LoadLevel(_levelList.CurrentLevelIndex);
    }

    [ContextMenu("Load Next Level")]
    public void LoadNextLevel()
    {
        LoadLevel(_levelList.CurrentLevelIndex + 1);
    }

    public void LoadLevel(int levelIndex)
    {
        if (levelIndex < 0)
        {
            levelIndex = 0;
            Logger.LogWarning($"Trying to load level with negative index ({levelIndex}), default to 0");
        }
        else if (levelIndex >= _levelList.LevelCount)
        {
            Logger.LogWarning($"Trying to load out of bounds level ({levelIndex}), default to last ({_levelList.LevelCount - 1})");
            levelIndex = _levelList.LevelCount - 1;
        }

        if (Application.isPlaying)
        {
            _levelList.SetCurrentLevelIndex(levelIndex);
            Logger.Log($"Loading level #{_levelList.CurrentLevelIndex}/{_levelList.LevelCount}");

            StartCoroutine(loadScene());

            IEnumerator loadScene()
            {
                if (_canvasFade)
                    _canvasFade.FadeOut();

                yield return new WaitForSeconds(0.35f);

                _onStartLoadingScene?.Invoke(_levelList.CurrentLevelIndex);
                var asyncLoadOperation = SceneManager.LoadSceneAsync(_levelList[_levelList.CurrentLevelIndex], LoadSceneMode.Single);
                asyncLoadOperation.completed += (asyncOperation) => _onSceneLoaded?.Invoke(_levelList.CurrentLevelIndex);
            }
        }
#if UNITY_EDITOR
        else
            EditorSceneManager.OpenScene(System.IO.Path.Combine(_levelList.ScenesFolderPath, _levelList[levelIndex]) + ".unity");
#endif
    }
    #endregion

    [ContextMenu("Refresh Levels List")]
    public void EditorRefreshLevelsList()
    {
        _levelList.EditorRefreshLevelsList();
    }

    public void IsInLevelLoadingMenu(bool state)
    {
        InLevelLoadingMenu?.Invoke(state);
    }

    public void QuitApp()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }
}
