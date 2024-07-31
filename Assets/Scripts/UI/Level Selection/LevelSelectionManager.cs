using UnityEngine;
using Eflatun.SceneReference;

namespace UI.LevelSelection
{
    public class LevelSelectionManager : MonoBehaviour
    {
        [SerializeField] TrainDoorAnimation doorAnimation;
        [SerializeField] LevelDetailsMenu levelDetailsMenu;
        [SerializeField] UIFadeAnimation exitSceneBackground;
        SceneReference selectedLevel;

        // Start is called before the first frame update
        void Start()
        {
            selectedLevel = null;
        }

        #region Main Level Selection Menu
        /// <summary>
        /// Select a level and go to detailed level selection scene
        /// </summary>
        /// <param name="index">Index of level in levels array</param>
        public void LevelSelected(int index)
        {
            doorAnimation?.PlayAnimation();
            levelDetailsMenu?.anim?.SetActive(true);

            if (GameManager.Instance == null)
            {
                Debug.LogWarning("Game manager instance is null, unable to show level details. (LevelSelectionManager.cs)");
                return;
            }

            selectedLevel = GameManager.Instance.config.levels[index].scene;
            levelDetailsMenu?.SetText(GameManager.Instance.config.levels[index].levelInfo.data.levelName, 
                GameManager.Instance.config.levels[index].levelInfo.data.levelDescription);
        }

        /// <summary>
        /// Return to previous scene (Main Menu)
        /// </summary>
        public void Back()
        {
            if (GameManager.Instance == null)
            {
                Debug.LogWarning("Game manager instance is null, \"Main Menu\" scene could not be loaded. (LevelSelectionManager.cs)");
                return;
            }
            GameManager.Instance.OpenScene_MainMenu();
        }
        #endregion

        #region Level Details Menu
        /// <summary>
        /// Return from detailed level selection scene to level selection scene
        /// </summary>
        public void LevelSelectionBack()
        {
            doorAnimation?.PlayAnimation();
            levelDetailsMenu?.anim?.SetActive(false);
            selectedLevel = null;
        }

        /// <summary>
        /// Load into currently selected level scene
        /// </summary>
        public void LoadLevel()
        {
            if (GameManager.Instance == null)
            {
                Debug.LogWarning("Game manager instance is null, level could not be loaded. (LevelSelectionManager.cs)");
                return;
            }

            if (selectedLevel == null) 
            {
                Debug.LogWarning("Selected level could not be found, returning to level selection scene. (LevelSelectionManager.cs)");
                LevelSelectionBack();
                return;
            }

            doorAnimation?.PlayAnimation();
            levelDetailsMenu?.anim?.SetActive(false);
            exitSceneBackground?.SetActive(true);
            GameManager.Instance.LoadLevel(selectedLevel.Name);
        }
        #endregion
    }
}

