using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

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
        if(Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        _currentLevelIndex = 0;
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

        if (Application.isPlaying)
        {
            _currentLevelIndex = levelIndex;
            Logger.Log($"Loading level #{_currentLevelIndex}/{_levels.Length}");

            _onStartLoadingScene?.Invoke(_currentLevelIndex);
            var asyncLoadOperation = SceneManager.LoadSceneAsync(_levels[_currentLevelIndex], LoadSceneMode.Single);
            asyncLoadOperation.completed += (asyncOperation) => _onSceneLoaded?.Invoke(_currentLevelIndex);
        }
#if UNITY_EDITOR
        else
            EditorSceneManager.OpenScene(System.IO.Path.Combine(_scenesFolderPath, _levels[levelIndex]) + ".unity");
#endif
    }

#if UNITY_EDITOR
    public void EditorRefreshLevelsList()
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
        EditorUtility.SetDirty(this);
    } 
#endif

    public void QuitApp()
    {
        Application.Quit();
    }
}
