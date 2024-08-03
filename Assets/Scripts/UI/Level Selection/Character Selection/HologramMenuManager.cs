using System.Collections;
using UnityEngine;
using UI.Animations;

namespace UI.LevelSelection.CharacterSelection
{
    public class HologramMenuManager : MonoBehaviour
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

        [Header("Transition Animation")]
        [SerializeField] float animationDuration = 1f;
        [SerializeField] float maxScale = 1f;

        [Header("Sprite Animation")]
        [SerializeField] UIAnimator anim;
        [SerializeField] Vector2 glitchCooldown;

        Coroutine coroutine_glitch_effect, coroutine_transition;
        
        public CharacterSelectProfileManager CharacterList => characterList;
        public CharacterInfoMenuManager CharacterInfo => characterInfo;
        public bool Active { get; private set; } = false;

        // Start is called before the first frame update
        void Start()
        {
            coroutine_glitch_effect = StartCoroutine(Glitch());
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
            coroutine_transition = StartCoroutine(AnimateTransition(active));
        }

        IEnumerator AnimateTransition(bool active)
        {
            float timeElasped = 0f;
            Vector3 scale = transform.localScale;

            if (active) scale.x = 0f;

            while (timeElasped < animationDuration)
            {
                transform.localScale = scale;
                scale.x = maxScale * (active ? (timeElasped / animationDuration) : 
                    1f - (timeElasped / animationDuration));
                timeElasped += Time.deltaTime;
                yield return timeElasped;
            }

            coroutine_transition = null;

            if (active)
            {
                scale.x = maxScale;
                transform.localScale = scale;
                if (coroutine_glitch_effect != null) StopCoroutine(coroutine_glitch_effect);
                coroutine_glitch_effect = StartCoroutine(Glitch());
            }
            else 
            {
                gameObject.SetActive(false);
            }
        }
        #endregion
        
        #region Sprite Animation
        IEnumerator Glitch()
        {
            yield return new WaitForSeconds(Random.Range(glitchCooldown.x, glitchCooldown.y));
            backButton?.SetActive(false);
            anim.Play("Glitch");
            yield return new WaitForSeconds(anim.currentAnimation.duration);
            backButton?.SetActive(true);
            anim.Play("Default");
            coroutine_glitch_effect = StartCoroutine(Glitch());
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
