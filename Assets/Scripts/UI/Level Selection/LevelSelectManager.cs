using System.Collections;
using UI.LevelSelection.CharacterSelection;
using UnityEngine;

public class LevelSelectManager : MonoBehaviour {

    [SerializeField]
    private LevelSelectHall levelSelectHall;
    [SerializeField]
    private CharacterSelectionManager characterSelect;
    public int selectedLevel = -1;
    bool launchingLevel = false;

    private void Start() {
        launchingLevel = false;
    }

    public void OpenLevelHallFor(LevelChoice choice) {
        levelSelectHall.Activate(GameManager.Instance.config.levels[choice.levelIndex].levelInfo);
    }
    
    public void OpenCharacterSelect() {
        characterSelect.OpenMenu();
    }

    public void LaunchLevel()
    {
        if (selectedLevel < 0) {
            Debug.LogError("No level selected!");
            return;
        }
        if (launchingLevel) return;
        launchingLevel = true;
        
        StartCoroutine(DelayedLaunchLevel(selectedLevel));
    }

    IEnumerator DelayedLaunchLevel(int levelIndex)
    {
        yield return levelSelectHall.LaunchAnimation();
        GameManager.Instance.LoadLevel(levelIndex);
    }
}