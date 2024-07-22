using System;
using Eflatun.SceneReference;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public struct GameLevel{
    public SceneReference scene;
    public LevelInfoSO levelInfo;
}


[CreateAssetMenu(fileName = "GameConfig", menuName = "ScriptableObjects/Game/GameConfig", order = 0)]
public class GameConfigSO : ScriptableObject
{
    public PlayerCharacterSO[] characters;
    public LevelScoreGradingSO grades;
    public GameLevel[] levels;
}