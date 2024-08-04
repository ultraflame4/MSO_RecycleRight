using System.Collections;
using UnityEngine;
using UI.Animations;

namespace UI.LevelSelection.CharacterSelection
{
    public class HologramMenuManager : Hologram
    {
        [Header("Menu Pages")]
        [SerializeField] CharacterSelectProfileManager characterList;
        [SerializeField] CharacterInfoMenuManager characterInfo;
        [SerializeField] PageState page_state = PageState.CHARACTER_INFO;
        public PageState pageState
        {
            get { return page_state; }
            set { 
                page_state = value; 
                UpdatePage(); 
            }
        }
        public enum PageState
        {
            CHARACTER_LIST, CHARACTER_INFO
        }

        [Header("UI")]
        [SerializeField] GameObject backButton;
        
        public CharacterSelectProfileManager CharacterList => characterList;
        public CharacterInfoMenuManager CharacterInfo => characterInfo;

        // Start is called before the first frame update
        void Start()
        {
            StartGlitch();
        }

        #region Active Management
        /// <summary>
        /// Set the active state of this game object
        /// </summary>
        /// <param name="active">Active state to set to</param>
        public void SetActive(bool active)
        {
            Active = active;

            if (active) 
            {
                gameObject.SetActive(true);
                backButton?.SetActive(true);
                characterList?.LoadCharacters();
                pageState = PageState.CHARACTER_LIST;
            }

            if (coroutine_transition != null) StopCoroutine(coroutine_transition);
            coroutine_transition = StartCoroutine(AnimateTransition(active, StartGlitch));
        }

        void StartGlitch()
        {
            if (coroutine_glitch_effect != null) StopCoroutine(coroutine_glitch_effect);
            coroutine_glitch_effect = StartCoroutine(Glitch(
                () => backButton?.SetActive(false), 
                () => backButton?.SetActive(true)
            ));
        }
        #endregion

        #region Menu Pages Management
        void UpdatePage()
        {
            characterList.gameObject.SetActive(pageState == PageState.CHARACTER_LIST);
            characterInfo.gameObject.SetActive(pageState != PageState.CHARACTER_LIST);
            if (pageState != PageState.CHARACTER_INFO) return;
            characterInfo.SetUI();
        }
        #endregion

        #region Button Event Handlers
        /// <summary>
        /// Show character info page
        /// </summary>
        public void ShowDetails()
        {
            pageState = PageState.CHARACTER_INFO;
        }
        
        /// <summary>
        /// Return to previous page
        /// </summary>
        public void Back()
        {
            pageState = PageState.CHARACTER_LIST;
        }
        #endregion
    }
}
