using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UI.Animations;

namespace UI.LevelSelection.CharacterSelection
{
    public class CharacterSelectionManager : MonoBehaviour
    {
        [Header("Menus")]
        [SerializeField] GameObject[] levelSelectionMenu;
        [SerializeField] GameObject[] characterSelectionMenu;

        [Header("Character Selection")]
        [SerializeField] HologramMenuManager hologramMenu;
        [SerializeField] Color[] selectionColor;
        [SerializeField] CharacterSelectSlot[] characterSlots;

        [Header("Transition")]
        [SerializeField] float transitionDuration = 1f;
        [SerializeField] GameObject transitionAnimation;
        Coroutine coroutine_transition;

        UIAnimator[] levelMenuAnimators, characterMenuAnimators;
        List<PlayerCharacterSO> party = new List<PlayerCharacterSO>();

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
            if (hologramMenu == null) return;
            hologramMenu.gameObject.SetActive(false);
            hologramMenu.CharacterList.CharacterProfileCreated += SubscribeToClick;
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

        #region Hologram Menu
        /// <summary>
        /// Set the active state of the hologram menu
        /// </summary>
        /// <param name="active">Active state to set to</param>
        public void SetHologramActive(bool active)
        {
            if (hologramMenu?.Active == active) return;
            hologramMenu?.SetActive(active);
            if (!active) return;
            UpdateSelectedCharactersUI();
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

            if (!active) return;
            UpdateSelectedCharactersUI();
            if (animators == null) return;

            foreach (UIAnimator anim in animators)
            {
                if (anim.currentAnimation == null || !anim.gameObject.activeSelf) continue;
                anim.Play(anim.currentAnimation.Name);
            }
        }
        #endregion

        #region Character Selection Management
        void SubscribeToClick(CharacterSelectProfile profile)
        {
            if (profile == null) return;
            profile.CharacterSelected += SelectCharacter;
        }

        void SelectCharacter(CharacterSelectProfile profile)
        {
            if (GameManager.Instance == null) return;

            hologramMenu?.CharacterInfo?.SetCharacter(profile.currentCharacter);

            if (party.Contains(profile.currentCharacter))
                party.Remove(profile.currentCharacter);
            else if (party.Count < GameManager.Instance.PartySize)
                party.Add(profile.currentCharacter);
            
            UpdateSelectedCharactersUI();
            GameManager.Instance.selectedCharacters = party.ToArray();
        }
        #endregion

        #region Character Selection UI Management
        void UpdateSelectedCharactersUI()
        {
            ResetCharacterSlot();
            SetBorder();
        }

        void SetBorder()
        {
            for (int i = 0; i < hologramMenu.CharacterList.objectPool.Count; i++)
            {
                CharacterSelectProfile profile = hologramMenu.CharacterList.objectPool[i];
                profile.HideBorder();
                if (!party.Contains(profile.currentCharacter)) continue;
                int index = party.FindIndex(x => x == profile.currentCharacter);
                if (index == -1) continue;
                profile.ShowBorder(selectionColor == null || selectionColor.Length <= index ? 
                    Color.white : selectionColor[index], index);
                UpdateCharacterSlot(index, profile.currentCharacter.characterSelectionSprite);
            }
        }

        void ResetCharacterSlot()
        {
            foreach (CharacterSelectSlot slot in characterSlots)
            {
                slot.SetCharacter();
            }
        }

        void UpdateCharacterSlot(int index, Sprite characterSprite)
        {
            if (characterSlots == null || index >= characterSlots.Length) return;
            characterSlots[index].SetCharacter(characterSprite);
        }
        #endregion
    }
}
