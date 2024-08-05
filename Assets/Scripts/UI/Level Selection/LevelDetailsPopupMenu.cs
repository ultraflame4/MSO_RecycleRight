using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UI.LevelSelection
{
    public class LevelDetailsPopupMenu : Hologram
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

        /// <summary>
        /// Set active state of menu
        /// </summary>
        /// <param name="active">Active state to set to</param>
        public void SetActive(bool active, int index)
        {
            if (active) gameObject.SetActive(active);
            if (!gameObject.activeInHierarchy) return;
            if (coroutine_transition != null) StopCoroutine(coroutine_transition);
            coroutine_transition = StartCoroutine(AnimateTransition(active, () => EndTransition(active, index)));
        }

        void EndTransition(bool active, int index)
        {
            if (!active || index < 0) return;
            SetButtonLocation(index);
        }

        /// <summary>
        /// Move button to location of popup menu
        /// </summary>
        /// <param name="index">Index of button position to move to</param>
        public void SetButtonLocation(int index)
        {
            if (levelButtons == null || index < 0 || index >= levelButtons.Length) return;
            map.localPosition = (-levelButtons[index].localPosition * map.localScale.x) + lockPosition;
        }

        void OnDrawGizmosSelected() 
        {
            if (canvas == null) return;
            Gizmos.DrawSphere(lockPosition + canvas.position, 100f);
        }
    }
}
