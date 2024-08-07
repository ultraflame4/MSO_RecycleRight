using UnityEngine;
using Eflatun.SceneReference;

namespace UI.LevelSelection
{
    public class LevelSelectionManager : MonoBehaviour
    {
        [SerializeField] TrainDoorAnimation doorAnimation;
        [SerializeField] LevelDetailsMenu levelDetailsMenu;
        [SerializeField] UIFadeAnimation skyBackground;
        [SerializeField] UIFadeAnimation exitSceneBackground;
        [SerializeField] LevelDetailsPopupMenu levelDetailsPopupMenu;
        [SerializeField] UIPointerHandler backgroundHandler;
        SceneReference selectedLevel;
        int selectedIndex = -1;

        // Start is called before the first frame update
        void Start()
        {
            selectedLevel = null;
            DeselectLevel();
            if (backgroundHandler == null) return;
            backgroundHandler.PointerDown += DeselectLevel;
        }

        #region Main Level Selection Menu
        /// <summary>
        /// Select a level and go to detailed level selection scene
        /// </summary>
        /// <param name="index">Index of level in levels array</param>
        public void LevelSelected(int index)
        {
            if (levelDetailsPopupMenu == null || levelDetailsPopupMenu.Active) return;

            selectedIndex = index;
            levelDetailsPopupMenu?.Activate(selectedIndex);

            if (selectedIndex < 0 || selectedIndex >= GameManager.Instance.config.levels.Length)
            {
                Debug.LogWarning($"Selected index {selectedIndex} is out of range {GameManager.Instance.config.levels.Length}, task has been aborted! (LevelSelectionManager.cs)");
                return;
            }

            if (GameManager.Instance == null)
            {
                Debug.LogWarning("Game manager instance is null, unable to show level details. (LevelSelectionManager.cs)");
                return;
            }

            levelDetailsPopupMenu?.SetDetails(GameManager.Instance.config.levels[selectedIndex].levelInfo);
        }

        /// <summary>
        /// Handle deselecting level
        /// </summary>
        public void DeselectLevel()
        {
            selectedIndex = -1;
            levelDetailsPopupMenu?.Activate(selectedIndex);
        }

        /// <summary>
        /// Show level details menu to prepare to start level
        /// </summary>
        public void ShowLevelDetails()
        {
            if (selectedIndex < 0 || selectedIndex >= GameManager.Instance.config.levels.Length)
            {
                Debug.LogWarning($"Selected index {selectedIndex} is out of range {GameManager.Instance.config.levels.Length}, task has been aborted! (LevelSelectionManager.cs)");
                return;
            }

            doorAnimation?.PlayAnimation();
            levelDetailsMenu?.SetActive(true);
            skyBackground?.SetActive(true);

            if (GameManager.Instance == null)
            {
                Debug.LogWarning("Game manager instance is null, unable to show level details. (LevelSelectionManager.cs)");
                return;
            }

            selectedLevel = GameManager.Instance.config.levels[selectedIndex].scene;
            levelDetailsMenu?.SetDetails(GameManager.Instance.config.levels[selectedIndex].levelInfo);
            DeselectLevel();
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
            levelDetailsMenu?.SetActive(false);
            skyBackground?.SetActive(false);
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
            levelDetailsMenu?.SetActive(false);
            exitSceneBackground?.SetActive(true);
            GameManager.Instance.LoadLevel(selectedLevel.Name);
        }
        #endregion
    }
}

