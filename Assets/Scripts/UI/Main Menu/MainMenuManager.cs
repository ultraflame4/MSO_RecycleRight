using UnityEngine;

namespace UI.MainMenu
{
    public class MainMenuManager : MonoBehaviour
    {
        GameManager gameManager => GameManager.Instance;

        /// <summary>
        /// Start game by opening level selection
        /// </summary>
        public void Play()
        {
            gameManager?.OpenScene_LevelSelection();
        }

        /// <summary>
        /// Quit the game
        /// </summary>
        public void Quit()
        {
            Debug.Log("Game Quit!");
            Application.Quit();
        }
    }
}
