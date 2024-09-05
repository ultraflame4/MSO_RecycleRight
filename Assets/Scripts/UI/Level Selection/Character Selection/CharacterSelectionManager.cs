using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UI.LevelSelection.CharacterSelection
{
    public class CharacterSelectionManager : MonoBehaviour
    {
        [Tooltip("Canvas group to hide when the menu is active")]
        [SerializeField] List<CanvasGroup> hideCanvasGroup = new();

        [Header("Character Selection")]
        [SerializeField] HologramMenuManager hologramMenu;
        [SerializeField] ToggleSwitch toggleQuickSelect;
        [SerializeField] Color[] selectionColor;
        [SerializeField] CharacterSelectSlot[] characterSlots;

        [Header("Transition")]
        [SerializeField] float transitionDuration = 1f;
        [SerializeField] CanvasGroup canvasGroup;
        [SerializeField] CharacterSelectionTransitionManager transitionAnimation;
        Coroutine coroutine_transition;
        List<PlayerCharacterSO> party = new List<PlayerCharacterSO>();

        int selectedIndex = -1;

        bool quickSelectActive => toggleQuickSelect != null && toggleQuickSelect.activated;
        public float transition_duration => transitionDuration;

        // Start is called before the first frame update
        void Start()
        {
            CloseMenu(true);
            if (toggleQuickSelect != null)
                toggleQuickSelect.OnToggle += ToggleHandler;

            if (hologramMenu == null) return;
            hologramMenu.gameObject.SetActive(false);
            hologramMenu.CharacterList.CharacterProfileCreated += SubscribeToClick;
        }

        #region Menu Toggling
        IEnumerator EnterTransition()
        {
            // handle showing transition
            transitionAnimation.gameObject.SetActive(true);
            yield return null;
            yield return transitionAnimation.HalfTransition(true, false);
            canvasGroup.alpha = 1;
            hideCanvasGroup.ToList().ForEach(x => x.alpha = 0);
            yield return transitionAnimation.HalfTransition(false, true);
            transitionAnimation?.gameObject.SetActive(false);
            coroutine_transition = null;
        }
        IEnumerator ExitTransition()
        {
            // handle showing transition
            transitionAnimation.gameObject.SetActive(true);
            yield return null;
            yield return transitionAnimation.HalfTransition(false, false);
            canvasGroup.alpha = 0;
            hideCanvasGroup.ToList().ForEach(x => x.alpha = 1);
            yield return transitionAnimation.HalfTransition(true, true);
            transitionAnimation?.gameObject.SetActive(false);
            coroutine_transition = null;
            gameObject.SetActive(false);
        }


        public void OpenMenu(bool skipTransition = false)
        {
            gameObject.SetActive(true);
            canvasGroup.alpha = 0;
            SetHologramActive(false);

            if (skipTransition)
            {
                hideCanvasGroup.ToList().ForEach(x => x.alpha = 0);
                return;
            }
            if (coroutine_transition != null) StopCoroutine(coroutine_transition);
            coroutine_transition = StartCoroutine(EnterTransition());
        }

        public void CloseMenu(bool skipTransition = false)
        {
            SetHologramActive(false);
            if (skipTransition)
            {
                gameObject.SetActive(false);
                hideCanvasGroup.ToList().ForEach(x => x.alpha = 1);
                return;
            }
            canvasGroup.alpha = 1;

            if (coroutine_transition != null) StopCoroutine(coroutine_transition);
            coroutine_transition = StartCoroutine(ExitTransition());
        }

        #endregion

        #region Button Event Handlers
        /// <summary>
        /// Set the active state of the hologram menu
        /// </summary>
        /// <param name="active">Active state to set to</param>
        public void SetHologramActive(bool active)
        {
            if (hologramMenu?.Active == active) return;
            hologramMenu?.SetActive(active);
            hologramMenu?.CharacterInfo?.SetCharacter(null);
            selectedIndex = -1;
            ResetLocalParty();
            UpdateCharacterSelectSlots();
        }

        /// <summary>
        /// apply party changes to 
        /// </summary>
        public void ConfirmSelection()
        {
            if (!quickSelectActive && hologramMenu.CharacterInfo.selectedCharacter != null)
            {
                if (party.Count > selectedIndex && selectedIndex >= 0)
                {
                    party.RemoveAt(selectedIndex);
                    party.Insert(selectedIndex, hologramMenu.CharacterInfo.selectedCharacter);
                }
                else
                {
                    party.Add(hologramMenu.CharacterInfo.selectedCharacter);
                }
            }

            UpdateCharacterSelectSlots();
            GameManager.Instance.selectedCharacters = party.ToArray();
        }

        /// <summary>
        /// return to previous page;
        /// </summary>
        public void Back()
        {
            if (hologramMenu.pageState == HologramMenuManager.PageState.CHARACTER_LIST)
                SetHologramActive(false);

            if (toggleQuickSelect != null && !toggleQuickSelect.activated)
                hologramMenu?.CharacterInfo?.SetCharacter(null);

            hologramMenu.Back();
        }

        /// <summary>
        /// Select specific slot in party to modify
        /// </summary>
        /// <param name="index">Index to select</param>
        public void SelectSlot(int index)
        {
            if (hologramMenu.pageState == HologramMenuManager.PageState.CHARACTER_INFO) return;
            selectedIndex = index;
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

            if (toggleQuickSelect != null && !toggleQuickSelect.activated)
                HandleDefaultSelect(profile);
            else
                HandleQuickSelect(profile);

            UpdateCharacterSelectSlots();
        }

        void HandleDefaultSelect(CharacterSelectProfile profile)
        {
            if (party.Contains(profile.currentCharacter))
            {
                int index = party.FindIndex(x => x == profile.currentCharacter);
                party.Remove(profile.currentCharacter);
                if (party.Count <= 0 || selectedIndex < 0 || index == selectedIndex) return;

                if (selectedIndex >= party.Count)
                    party.Add(profile.currentCharacter);
                else
                    party.Insert(selectedIndex, profile.currentCharacter);
            }

            hologramMenu?.CharacterInfo?.SetCharacter(profile.currentCharacter);
            hologramMenu?.ShowDetails();
        }

        void HandleQuickSelect(CharacterSelectProfile profile)
        {
            // remove viewing details feature in quick select as it is unintuitive
            // if (hologramMenu != null && hologramMenu.CharacterInfo.selectedCharacter == profile.currentCharacter)
            // {
            //     hologramMenu.CharacterInfo?.SetCharacter(null);
            //     return;
            // }

            if (party.Contains(profile.currentCharacter))
            {
                party.Remove(profile.currentCharacter);
                hologramMenu.CharacterInfo.SetCharacter(profile.currentCharacter);
                return;
            }

            if (party.Count < GameManager.Instance.PartySize)
            {
                party.Add(profile.currentCharacter);
            }
        }

        void ToggleHandler()
        {
            // if (toggleQuickSelect != null && !toggleQuickSelect.activated)
            //     ResetLocalParty();
            hologramMenu?.CharacterInfo?.SetCharacter(null);
            UpdateCharacterSelectSlots();
        }
        #endregion

        #region Character Selection UI Management
        void UpdateCharacterSelectSlots()
        {
            for (int i = 0; i < characterSlots.Length; i++)
            {
                UpdateCharacterSlot(i, i < party.Count ? party[i] : null);
            }

            UpdateProfileBorder();
        }

        void UpdateProfileBorder()
        {
            for (int i = 0; i < hologramMenu.CharacterList.objectPool.Count; i++)
            {
                CharacterSelectProfile profile = hologramMenu.CharacterList.objectPool[i];
                if (!profile.gameObject.activeSelf) continue;
                profile.HideBorder();

                // remove viewing details feature in quick select as it is unintuitive
                // if (toggleQuickSelect != null && toggleQuickSelect.activated)
                //     profile.SetSelection(profile.currentCharacter == hologramMenu.CharacterInfo.selectedCharacter);
                // else
                //     profile.SetSelection(false);

                if (!party.Contains(profile.currentCharacter)) continue;

                int index = party.FindIndex(x => x == profile.currentCharacter);
                if (index == -1) continue;

                profile.ShowBorder(selectionColor == null || selectionColor.Length <= index ?
                    Color.white : selectionColor[index], index);

                UpdateCharacterSlot(index, profile.currentCharacter);
            }
        }
        void UpdateCharacterSlot(int index, PlayerCharacterSO character)
        {
            if (characterSlots == null || index >= characterSlots.Length) return;
            characterSlots[index].SetCharacter(character);
        }

        void ResetCharacterSlot()
        {
            foreach (CharacterSelectSlot slot in characterSlots)
            {
                slot.SetCharacter();
            }
        }

        void ResetLocalParty()
        {
            if (GameManager.Instance == null || GameManager.Instance.selectedCharacters == null)
                party.Clear();
            else
                party = GameManager.Instance.selectedCharacters.ToList();
        }
        #endregion
    }
}
