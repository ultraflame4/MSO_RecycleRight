using UnityEngine;

public struct Character
{
    public GameObject prefab;
    public Entity.Data.PlayerCharacter data;
}

public struct GameLevel{
    public string scene_name;
    public LevelInfoSO levelInfo;
}


[CreateAssetMenu(fileName = "GameConfig", menuName = "ScriptableObjects/Game/GameConfig", order = 0)]
public class GameConfigSO : ScriptableObject
{
    public PlayerCharacterSO[] characters;
    public LevelScoreGradingSO grades;
    public GameLevel[] levels;
}