using System.Collections;
using UI.LevelSelection.CharacterSelection;
using UnityEngine;

public class LevelSelectManager : MonoBehaviour 
{
    [Header("Level Select")]
    [SerializeField] private LevelSelectHall levelSelectHall;
    [SerializeField] private CharacterSelectionManager characterSelect;
    public int selectedLevel = -1;

    [Header("Error Handling")]
    [SerializeField] GameObject errorText;
    [SerializeField] float errorShowDuration = 2.5f;

    bool launchingLevel = false;

    private void Start() 
    {
        launchingLevel = false;
        errorText?.SetActive(false);

        // reset to no selected characters whenever entering level selection
        if (GameManager.Instance == null)
        {
            Debug.LogWarning("Game Manager instance could not be found, party could not be resetted! (LevelSelectManager.cs)");
            return;
        }
        GameManager.Instance.selectedCharacters = null;
    }

    public void OpenLevelHallFor(LevelChoice choice) 
    {
        levelSelectHall.Activate(GameManager.Instance.config.levels[choice.levelIndex].levelInfo);
    }
    
    public void OpenCharacterSelect() 
    {
        characterSelect.OpenMenu();
    }

    public void LaunchLevel()
    {
        if (selectedLevel < 0) 
        {
            Debug.LogError("No level selected!");
            return;
        }

        // if try to start level without selecting characters, open level select
        if (GameManager.Instance.selectedCharacters == null || GameManager.Instance.selectedCharacters.Length <= 0)
        {
            OpenCharacterSelect();
            StartCoroutine(ShowErrorText());
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

    IEnumerator ShowErrorText()
    {
        yield return new WaitForSeconds(characterSelect.transition_duration);
        errorText?.SetActive(true);
        yield return new WaitForSeconds(errorShowDuration);
        errorText?.SetActive(false);
    }
}