using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI.MainMenu
{
    public class MainMenuManager : MonoBehaviour
    {
        GameManager gameManager => GameManager.Instance;

        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            
        }

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
