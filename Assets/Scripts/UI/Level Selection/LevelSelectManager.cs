using UI.LevelSelection.CharacterSelection;
using UnityEngine;

public class LevelSelectManager : MonoBehaviour {

    [SerializeField]
    private LevelSelectHall levelSelectHall;
    [SerializeField]
    private CharacterSelectionManager characterSelect;
    public int selectedLevel = -1;

    public void OpenLevelHallFor(LevelChoice choice) {
        Debug.Log($"Opening level hall for level index: {choice.levelIndex}");
        levelSelectHall.Activate(GameManager.Instance.config.levels[choice.levelIndex].levelInfo);
    }
    
    public void OpenCharacterSelect() {
        Debug.Log("Opening character select");
        characterSelect.OpenMenu();
    }

    public void LaunchLevel()
    {
        if (selectedLevel < 0) {
            Debug.LogError("No level selected!");
            return;
        }
        Debug.Log("Launching level");
        GameManager.Instance.LoadLevel(selectedLevel);
    }
}