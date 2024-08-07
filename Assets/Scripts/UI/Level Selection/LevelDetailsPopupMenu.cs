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

        IEnumerator MoveButtonToCenter(int btnIndex)
        {
            while (true)
            {
                var dest = (-levelButtons[btnIndex].localPosition * map.localScale.x) + lockPosition;
                var distance = Vector3.Distance(dest, map.localPosition);
                map.localPosition = Vector3.Lerp(map.localPosition, dest, Time.deltaTime * 10);
                if (distance < 0.1f) break;
                yield return null;

            }
        }

        IEnumerator PlayOpeningForButton(int btnIndex)
        {
            yield return AnimateClose();
            // Move button location
            yield return MoveButtonToCenter(btnIndex);
            yield return AnimateOpen();
        }

        /// <summary>
        /// Set active state of menu
        /// </summary>
        /// <param name="active">Active state to set to</param>
        public void Activate(int index)
        {
            gameObject.SetActive(true);// if trying to active, ensure that gameobject is alr active
            
            if (coroutine_transition != null) StopCoroutine(coroutine_transition);
            coroutine_transition = StartCoroutine(PlayOpeningForButton(index));
        }

        /// <summary>
        /// Closes the popup menu.
        /// </summary>
        public void Deactivate()
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
            if (Input.GetMouseButtonDown(0) && !mouseHover) Deactivate();
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
