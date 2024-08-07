using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections;

namespace UI.LevelSelection
{
    public class LevelDetailsPopupMenu : Hologram, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("Popup Menu")]
        [SerializeField] Image image;
        [SerializeField] TextMeshProUGUI levelName, levelCode;

        [Header("On Click Behaviour")]
        [SerializeField] Vector3 lockPosition;
        [SerializeField] RectTransform map, canvas;
        [SerializeField] RectTransform[] levelButtons;

        int targetLevelButton;

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

        /// <summary>
        /// Set active state of menu
        /// </summary>
        /// <param name="active">Active state to set to</param>
        public void Activate(int index)
        {
            gameObject.SetActive(true);// if trying to active, ensure that gameobject is alr active
            if (coroutine_transition != null) StopCoroutine(coroutine_transition);
            coroutine_transition = StartCoroutine(AnimateTransition(true, () => MakeButtonCenter(index)));
        }

        /// <summary>
        /// Closes the popup menu.
        /// </summary>
        public void Deactivate()
        {
            if (!gameObject.activeInHierarchy) return; // Silences error
            if (coroutine_transition != null) StopCoroutine(coroutine_transition);
            coroutine_transition = StartCoroutine(AnimateTransition(false));
            targetLevelButton = -1;
        }

        /// <summary>
        /// Move the level select button to location of popup menu
        /// </summary>
        /// <param name="btnIndex">Index of button position to move to</param>
        public void MakeButtonCenter(int btnIndex)
        {
            targetLevelButton = btnIndex;
        }

        void OnDrawGizmosSelected()
        {
            if (canvas == null) return;
            Gizmos.DrawSphere(lockPosition + canvas.position, 100f);
        }


        private void Update()
        {
            if (Input.GetMouseButtonDown(0) && !mouseHover) Deactivate();

            if (levelButtons == null || targetLevelButton < 0 || targetLevelButton >= levelButtons.Length) return;
            var dest = (-levelButtons[targetLevelButton].localPosition * map.localScale.x) + lockPosition;
            var distance = Vector3.Distance(dest, map.localPosition);
            map.localPosition = Vector3.Lerp(map.localPosition, dest, Time.deltaTime * 10);
            if (distance < 0.1f && targetLevelButton >= 0){
                
                targetLevelButton = -1;
            }
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
    }
}
