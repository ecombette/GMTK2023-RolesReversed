using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

public class GameManager : MonoBehaviour
{
    [Header("Levels")]
    [SerializeField] private LevelList _levelList;
    [SerializeField] UICanvasFade _canvasFade;
    [SerializeField] private UnityEvent<int> _onStartLoadingScene, _onSceneLoaded;

    [ContextMenu("Load First Level")]
    public void LoadFirstLevel()
    {
        LoadLevel(0);
    }

    [ContextMenu("Load Current Level")]
    public void LoadCurrentLevel()
    {
        LoadLevel(_levelList._currentLevelIndex);
    }

    [ContextMenu("Load Next Level")]
    public void LoadNextLevel()
    {
        LoadLevel(_levelList._currentLevelIndex + 1);
    }

    public void LoadLevel(int levelIndex)
    {
        if(levelIndex < 0)
        {
            levelIndex = 0;
            Logger.LogWarning($"Trying to load level with negative index ({levelIndex}), default to 0");
        }
        else if(levelIndex >= _levelList._levels.Length)
        {
            Logger.LogWarning($"Trying to load out of bounds level ({levelIndex}), default to last ({_levelList._levels.Length - 1})");
            levelIndex = _levelList._levels.Length - 1;
        }

        if (Application.isPlaying)
        {
            _levelList._currentLevelIndex = levelIndex;
            Logger.Log($"Loading level #{_levelList._currentLevelIndex}/{_levelList._levels.Length}");

            StartCoroutine(loadScene());

            IEnumerator loadScene()
            {
                _canvasFade.FadeOut();

                yield return new WaitForSeconds(0.35f);

                _onStartLoadingScene?.Invoke(_levelList._currentLevelIndex);
                var asyncLoadOperation = SceneManager.LoadSceneAsync(_levelList._levels[_levelList._currentLevelIndex], LoadSceneMode.Single);
                asyncLoadOperation.completed += (asyncOperation) => _onSceneLoaded?.Invoke(_levelList._currentLevelIndex);
            }
        }
#if UNITY_EDITOR
        else
            EditorSceneManager.OpenScene(System.IO.Path.Combine(_levelList._scenesFolderPath, _levelList._levels[levelIndex]) + ".unity");
#endif
    }

#if UNITY_EDITOR
    [ContextMenu("Refresh Levels List")]
    public void EditorRefreshLevelsList()
    {
        List<EditorBuildSettingsScene> scenesToBuild = new List<EditorBuildSettingsScene>();

        var sceneGuids = AssetDatabase.FindAssets("t:Scene", new[] { _levelList._scenesFolderPath });

        _levelList._levels = new string[sceneGuids.Length];
        for(int i = 0; i < sceneGuids.Length; i++)
        {
            var scenePath = AssetDatabase.GUIDToAssetPath(sceneGuids[i]);
            EditorBuildSettingsScene scene = new EditorBuildSettingsScene();
            scene.path = scenePath;
            scene.enabled = true;
            scenesToBuild.Add(scene);

            _levelList._levels[i] = System.IO.Path.GetFileNameWithoutExtension(scenePath);
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
