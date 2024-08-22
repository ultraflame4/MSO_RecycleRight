using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections;

namespace UI.LevelSelection
{
    public class LevelDetailsPopupMenu : Hologram, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] MapLevelSelect mapSelect;
        [Header("Popup Menu")]
        [SerializeField] Image image;
        [SerializeField] TextMeshProUGUI levelName;
        [SerializeField] TextMeshProUGUI levelCode;

        [Header("On Click Behaviour")]
        [SerializeField] Vector3 lockPosition;
        [SerializeField] RectTransform canvas;
        [SerializeField] RectTransform map;
        LevelChoice currentChoice;

        void Awake()
        {
            var localScale = transform.localScale;
            localScale.x = 0;
            transform.localScale = localScale;
        }
        /// <summary>
        /// Set details of level to show
        /// </summary>
        /// <param name="data">Level data</param>
        public void SetDetails(LevelInfoSO data)
        {
            if (data == null) return;
            Sprite sprite = data.data.levelImage;
            if (sprite != null) image.sprite = sprite;
            levelName.text = data.data.levelName;
            levelCode.text = data.data.levelCode;
        }

        float MoveButtonToCenter_Step(LevelChoice btn)
        {
            var dest = lockPosition - btn.transform.localPosition * map.localScale.x;
            map.localPosition = Vector3.Lerp(map.localPosition, dest, Time.deltaTime * 10);
            var distance = Vector3.Distance(dest, map.localPosition);
            return distance;
        }

        IEnumerator PlayOpeningForButton(LevelChoice btn)
        {
            yield return AnimateClose();
            Coroutine earlyOpen = null;

            while (true)
            {
                var d = MoveButtonToCenter_Step(btn);
                if (d < 40f && earlyOpen == null)
                {
                    earlyOpen = StartCoroutine(AnimateOpen());
                }
                if (d < 0.1f) break;
                yield return null;
            }

            yield return earlyOpen;
        }

        /// <summary>
        /// Set active state of menu
        /// </summary>
        /// <param name="active">Active state to set to</param>
        public void ShowForLevelBtn(LevelChoice btn)
        {
            gameObject.SetActive(true);// if trying to active, ensure that gameobject is alr active
            // Retrieve level details from game manager and set to popup menu
            
            if (GameManager.Instance.config.levels.Length > btn.levelIndex){
                SetDetails(GameManager.Instance.config.levels[btn.levelIndex].levelInfo);
            }
            else{
                Debug.LogWarning($"Could not get level details - No level found at index: {btn.levelIndex} in the GameManager config levels");
            }

            currentChoice = btn;
            if (coroutine_transition != null) StopCoroutine(coroutine_transition);
            coroutine_transition = StartCoroutine(PlayOpeningForButton(btn));
        }

        /// <summary>
        /// Closes the popup menu.
        /// </summary>
        public void Hide()
        {
            if (!gameObject.activeInHierarchy) return; // Silences error
            if (coroutine_transition != null) StopCoroutine(coroutine_transition);
            coroutine_transition = StartCoroutine(AnimateClose());
        }

        void OnDrawGizmosSelected()
        {
            if (canvas == null) return;
            Gizmos.DrawSphere(lockPosition + canvas.position, 100f);
        }


        private void Update()
        {
            if (Input.GetMouseButtonDown(0) && !mouseHover) Hide();
        }

        bool mouseHover = false;

        public void OnPointerExit(PointerEventData eventData)
        {
            mouseHover = false;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            mouseHover = true;
        }

        public void OpenLevelChoiceHall()
        {
            mapSelect.OpenLevelChoiceHall(currentChoice);
        }
    }
}
