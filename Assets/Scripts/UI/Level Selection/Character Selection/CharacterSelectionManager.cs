using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UI.Animations;

namespace UI.LevelSelection.CharacterSelection
{
    public class CharacterSelectionManager : MonoBehaviour
    {
        [SerializeField] GameObject[] levelSelectionMenu;
        [SerializeField] GameObject[] characterSelectionMenu;

        [Header("Transition")]
        [SerializeField] float transitionDuration = 1f;
        [SerializeField] GameObject transitionAnimation;
        Coroutine coroutine_transition;

        UIAnimator[] levelMenuAnimators, characterMenuAnimators;

        /// <summary>
        /// Active state of character selection
        /// </summary>
        public bool MenuActive { get; private set; } = false;

        // Start is called before the first frame update
        void Start()
        {
            LoadAnimators(levelSelectionMenu, out levelMenuAnimators);
            LoadAnimators(characterSelectionMenu, out characterMenuAnimators);
            SetMenuActive(true, levelSelectionMenu, levelMenuAnimators);
            SetMenuActive(false, characterSelectionMenu, characterMenuAnimators);
            transitionAnimation?.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        #region Menu Toggling
        /// <summary>
        /// Toggle active state of character selection menu
        /// </summary>
        public void ToggleMenu()
        {
            MenuActive = !MenuActive;
            if (coroutine_transition != null) StopCoroutine(coroutine_transition);
            coroutine_transition = StartCoroutine(Transition());
        }

        IEnumerator Transition()
        {
            // handle showing transition
            transitionAnimation?.SetActive(true);
            yield return new WaitForSeconds(transitionDuration);
            transitionAnimation?.SetActive(false);
            coroutine_transition = null;
            // set menu renderers
            SetMenuActive(!MenuActive, levelSelectionMenu, levelMenuAnimators);
            SetMenuActive(MenuActive, characterSelectionMenu, characterMenuAnimators);
        }
        #endregion

        #region Active Management
        void LoadAnimators(GameObject[] objectsToSearch, out UIAnimator[] animators)
        {
            if (objectsToSearch == null) 
            {
                animators = null;
                return;
            }

            animators = objectsToSearch
                .Select(x => x.GetComponentsInChildren<UIAnimator>()
                    .Where(x => x != null))
                .SelectMany(x => x)
                .ToArray();
        }

        void SetMenuActive(bool active, GameObject[] objects, UIAnimator[] animators)
        {
            if (objects == null) return;

            foreach (GameObject obj in objects)
            {
                obj?.SetActive(active);
            }

            if (!active || animators == null) return;

            foreach (UIAnimator anim in animators)
            {
                if (anim.currentAnimation == null) continue;
                anim.Play(anim.currentAnimation.Name);
            }
        }
        #endregion
    }
}
