using System.Collections.Generic;
using UI.LevelSelection;
using UnityEngine;

public class MapLevelSelect : MonoBehaviour {

    [SerializeField]
    private LevelSelectManager lvlSelect;
    [SerializeField]
    private LevelDetailsPopupMenu lvlDetailsPopup;
    private List<LevelChoice> levelButtons = new();

    private void Start() 
    {
        LevelChoice[] array = GetComponentsInChildren<LevelChoice>();

        for (int i = 0; i < array.Length; i++) 
        {
            LevelChoice button = array[i];
            button.OnClicked += () => lvlDetailsPopup.ShowForLevelBtn(button);
        }
    }

    public void OpenLevelChoiceHall(LevelChoice choice)
    {
        lvlSelect.selectedLevel = choice.levelIndex;
        lvlSelect.OpenLevelHallFor(choice);
    }

    public void Back()
    {
        GameManager.Instance?.OpenScene_MainMenu();
    }
}