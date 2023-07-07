using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GameManager gameManager = (GameManager)target;

        if(GUILayout.Button("Load First Level"))
            gameManager.LoadFirstLevel();
        if(GUILayout.Button("Load Next Level"))
            gameManager.LoadNextLevel();
        if(GUILayout.Button("Load Current Level"))
            gameManager.LoadCurrentLevel();

        GUILayout.Space(15);

        if (GUILayout.Button("Refresh Levels List"))
            gameManager.EditorRefreshLevelsList();
    }
}
