using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UI.LevelSelection
{
    public class LevelDetailsPopupMenu : MonoBehaviour
    {
        [SerializeField] GameObject menu;
        [SerializeField] Image image;
        [SerializeField] TextMeshProUGUI levelName, levelCode;
        [SerializeField] float transitionDuration = 1f;
        Coroutine coroutine_transition;

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
        public void SetActive(bool active)
        {
            menu?.SetActive(active);
        }

        IEnumerator Transition(bool active)
        {
            float timeElasped = 0f;
            Vector3 scale = menu.transform.localScale;
            scale.x = active ? 0f : 1f;

            while (timeElasped < transitionDuration)
            {
                timeElasped += Time.deltaTime;
                yield return timeElasped;
            }
        }
    }
}
