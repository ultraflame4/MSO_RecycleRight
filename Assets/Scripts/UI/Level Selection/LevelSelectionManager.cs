using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI.LevelSelection
{
    public class LevelSelectionManager : MonoBehaviour
    {
        [SerializeField] TrainDoorAnimation doorAnimation;
        [SerializeField] DetailedLevelMenu levelDetailsMenu;

        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        #region Level Selection Menu Button Events
        public void LevelSelected(int index)
        {
            doorAnimation?.PlayAnimation();
            levelDetailsMenu?.SetActive(true);
        }

        public void LevelSelectionBack()
        {
            doorAnimation?.PlayAnimation();
            levelDetailsMenu?.SetActive(false);
        }
        #endregion

        #region Game Manager Button Events
        /// <summary>
        /// Load level scene based on the name of the scene
        /// </summary>
        /// <param name="name">Name of level scene to load</param>
        public void LoadLevel(string name)
        {
            if (GameManager.Instance == null)
            {
                Debug.LogWarning("Game manager instance is null, level could not be loaded. (LevelSelectionManager.cs)");
                return;
            }
            GameManager.Instance.LoadLevel(name);
        }

        /// <summary>
        /// Load level scene based on index in Level Names array
        /// </summary>
        /// <param name="index">Index of level name in array</param>
        public void LoadLevel(int index)
        {
            if (GameManager.Instance == null)
            {
                Debug.LogWarning("Game manager instance is null, level could not be loaded. (LevelSelectionManager.cs)");
                return;
            }
            GameManager.Instance.LoadLevel(index);
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
    }
}

