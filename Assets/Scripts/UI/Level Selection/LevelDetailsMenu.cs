using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UI.LevelSelection
{
    public class LevelDetailsMenu : Hologram
    {
        [Header("Menu Details")]
        [SerializeField] Image image;
        [SerializeField] TextMeshProUGUI levelNameText, levelDescText;
        [SerializeField] UIFadeAnimation fadeAnim;
        Coroutine coroutine;

        /// <summary>
        /// Set details of level to show
        /// </summary>
        /// <param name="data">Level data</param>
        public void SetDetails(LevelInfoSO data)
        {
            if (data == null) return;
            if (image != null) image.sprite = data.data.levelImage;
            if (levelNameText != null) levelNameText.text = data.data.levelName;
            if (levelDescText != null) levelDescText.text = data.data.levelDescription;
        }

        /// <summary>
        /// Set active state of menu
        /// </summary>
        /// <param name="active">Active state to set to</param>
        public void SetActive(bool active)
        {
            fadeAnim?.SetActive(active);
            if (!active) return;
            if (coroutine != null) StopCoroutine(coroutine);
            coroutine = StartCoroutine(DelayedAnimation());
        }

        IEnumerator DelayedAnimation()
        {
            yield return new WaitForSeconds(fadeAnim.FadeDelay + fadeAnim.FadeDuration);
            if (coroutine_glitch_effect != null) StopCoroutine(coroutine_glitch_effect);
            coroutine_glitch_effect = StartCoroutine(Glitch());
        }
    }
}
