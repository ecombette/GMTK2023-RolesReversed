using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(menuName = "LevelList", fileName = "LevelList")]
public class LevelList : ScriptableObject
{
    [SerializeField] string[] _levels;
    [SerializeField] string _scenesFolderPath = "Assets/Scenes";

    public int CurrentLevelIndex = 0;

    public int LevelCount => _levels.Length;
    public string ScenesFolderPath => _scenesFolderPath;

    public string this[int levelIndex]
    {
        get
        {
            if (levelIndex < 0 || levelIndex > _levels.Length)
            {
                Logger.LogError($"Trying to access level #{levelIndex} but there are only {_levels.Length} levels");
                return string.Empty;
            }

            return _levels[levelIndex];
        }
    }

#if UNITY_EDITOR
    public void EditorTrySetCurrentLevel(string sceneName)
    {
        int levelIndex = System.Array.IndexOf(_levels, sceneName);
        if (levelIndex > 0)
            CurrentLevelIndex = levelIndex;
    } 
#endif

    [ContextMenu("Refresh Levels List")]
    public void EditorRefreshLevelsList()
    {
#if UNITY_EDITOR
        List<EditorBuildSettingsScene> scenesToBuild = new List<EditorBuildSettingsScene>();

        var sceneGuids = AssetDatabase.FindAssets("t:Scene", new[] { _scenesFolderPath });

        _levels = new string[sceneGuids.Length];
        for (int i = 0; i < sceneGuids.Length; i++)
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
#endif
    }
}
