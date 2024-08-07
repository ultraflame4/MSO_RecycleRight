using UI.LevelSelection.CharacterSelection;
using UnityEngine;

public class LevelSelectManager : MonoBehaviour {

    [SerializeField]
    private LevelSelectHall levelSelectHall;
    [SerializeField]
    private CharacterSelect charSelect;

    public void OpenLevelHallFor(LevelChoice choice) {
        Debug.Log($"Opening level hall for level index: {choice.levelIndex}");
        levelSelectHall.Activate(GameManager.Instance.config.levels[choice.levelIndex].levelInfo);
    }
    
    public void OpenCharacterSelect() {
        Debug.Log("Opening character select");
        charSelect.Activate();
    }

}