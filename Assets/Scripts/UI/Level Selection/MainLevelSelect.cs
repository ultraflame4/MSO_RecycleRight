using System.Collections.Generic;
using UI.LevelSelection;
using UnityEngine;

public class MainLevelSelect : MonoBehaviour {

    [SerializeField]
    private LevelSelectManager lvlSelect;
    [SerializeField]
    private LevelDetailsPopupMenu lvlDetailsPopup;
    private List<LevelButton> levelButtons = new();

    private void Start() {
        LevelButton[] array = GetComponentsInChildren<LevelButton>();
        for (int i = 0; i < array.Length; i++) {
            LevelButton button = array[i];
            button.OnClicked += () => {
                lvlDetailsPopup.ShowForLevelBtn(button);
                Debug.Log($"Opening level details popup for level index: {button.levelIndex}");
            };
        }
    }

    
}