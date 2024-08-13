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
        if (launchingLevel) return;
        launchingLevel = true;
        Debug.Log("Launching level");
        levelSelectHall.TriggerLaunchAnimations();
        StartCoroutine(DelayedLaunchLevel(selectedLevel));
    }

    IEnumerator DelayedLaunchLevel(int levelIndex)
    {
        yield return new WaitForSeconds(2.5f);
        GameManager.Instance.LoadLevel(levelIndex);
    }
}