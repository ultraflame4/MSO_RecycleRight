using UnityEngine;

public class LevelSelectManager : MonoBehaviour {

    [SerializeField]
    private LevelSelectHall levelSelectHall;

    public void OpenLevelHallFor(LevelChoice choice) {
        Debug.Log($"Opening level hall for level index: {choice.levelIndex}");
        levelSelectHall.Activate(GameManager.Instance.config.levels[choice.levelIndex].levelInfo);
    }
    
}