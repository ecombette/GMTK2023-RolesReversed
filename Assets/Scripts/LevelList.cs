using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "LevelList", fileName = "LevelList")]
public class LevelList : ScriptableObject
{
    public string[] _levels;
    public string _scenesFolderPath = "Assets/Scenes";
    public int _currentLevelIndex = 0;
}
