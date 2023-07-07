#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor; 
#endif
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Levels")]
    [SerializeField]
    private string[] _levels;
    [SerializeField]
    private string _scenesFolderPath = "Assets/Scenes";
    [SerializeField]
    private int _currentLevelIndex = 0;
    [SerializeField]
    private UnityEvent<int> _onStartLoadingScene, _onSceneLoaded;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void LoadFirstLevel()
    {
        LoadLevel(0);
    }

    public void LoadCurrentLevel()
    {
        LoadLevel(_currentLevelIndex);
    }

    public void LoadNextLevel()
    {
        LoadLevel(_currentLevelIndex + 1);
    }

    public void LoadLevel(int levelIndex)
    {
        if(levelIndex < 0)
        {
            levelIndex = 0;
            Logger.LogWarning($"Trying to load level with negative index ({levelIndex}), default to 0");
        }
        else if(levelIndex >= _levels.Length)
        {
            Logger.LogWarning($"Trying to load out of bounds level ({levelIndex}), default to last ({_levels.Length - 1})");
            levelIndex = _levels.Length - 1;
        }

        _currentLevelIndex = levelIndex;
        Logger.Log($"Loading level #{_currentLevelIndex}/{_levels.Length}");

        _onStartLoadingScene?.Invoke(_currentLevelIndex);
        var asyncLoadOperation = SceneManager.LoadSceneAsync(_levels[_currentLevelIndex], LoadSceneMode.Single);
        asyncLoadOperation.completed += (asyncOperation) => _onSceneLoaded?.Invoke(_currentLevelIndex);
    }

#if UNITY_EDITOR
    public void EdtiorRefreshLevelsList()
    {
        List<EditorBuildSettingsScene> scenesToBuild = new List<EditorBuildSettingsScene>();

        var sceneGuids = AssetDatabase.FindAssets("t:Scene", new[] { _scenesFolderPath });
        _levels = new string[sceneGuids.Length];
        for(int i = 0; i < sceneGuids.Length; i++)
        {
            var scenePath = AssetDatabase.GUIDToAssetPath(sceneGuids[i]);
            EditorBuildSettingsScene scene = new EditorBuildSettingsScene();
            scene.path = scenePath;
            scene.enabled = true;
            scenesToBuild.Add(scene);

            _levels[i] = System.IO.Path.GetFileNameWithoutExtension(scenePath);
        }

        EditorBuildSettings.scenes = scenesToBuild.ToArray();
    } 
#endif
}
