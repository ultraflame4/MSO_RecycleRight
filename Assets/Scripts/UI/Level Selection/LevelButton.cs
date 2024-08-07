using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Utility class for automatic grouping of level buttons
/// </summary>
public class LevelButton : MonoBehaviour {

    public int levelIndex;
    public Button[] buttons;
    public event Action OnClicked;

    // todo automatically set level text with information retrieved from LevelInfoSO (in game manager)

    private void Start() {
        foreach (var button in buttons) {
            button.onClick.AddListener(() => OnClicked?.Invoke());
        }
    }
}