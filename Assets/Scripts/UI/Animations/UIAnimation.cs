using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Animations
{
    public class UIAnimation : MonoBehaviour
    {
        [SerializeField] string animationName;
        [SerializeField] float durationBetweenFrames = 100f;
        [SerializeField] Image targetImage;
        [SerializeField] Sprite[] sprites;

        Coroutine playAnimationCoroutine;
        int currentSprite = 0;

        public string Name => animationName;
        public float duration => sprites == null ? 0f : (durationBetweenFrames / 1000f) * sprites.Length;

        /// <summary>
        /// Play animation
        /// </summary>
        public void Play()
        {
            // ensure required fields are not null
            if (targetImage == null || sprites.Length <= 0)
            {
                Debug.LogError("Target Image or required sprites are not set. (UIAnimation.cs)");
                return;
            }

            // reset current sprite
            currentSprite = 0;
            // ensure coroutine is null
            if (playAnimationCoroutine != null) StopCoroutine(playAnimationCoroutine);
            // play animation
            playAnimationCoroutine = StartCoroutine(PlayAnimation());
        }

        /// <summary>
        /// Stop playing animation
        /// </summary>
        public void Stop()
        {
            // ensure required fields are not null
            if (targetImage == null || sprites.Length <= 0)
            {
                Debug.LogError("Target Image or required sprites are not set. (UIAnimation.cs)");
                return;
            }
            // ensure coroutine is not null
            if (playAnimationCoroutine == null) return;
            // stop animation coroutine
            StopCoroutine(playAnimationCoroutine);
            // reset coroutine variable to null
            playAnimationCoroutine = null;
            // reset sprite to first sprite in animation
            targetImage.sprite = sprites[0];
        }

        IEnumerator PlayAnimation()
        {
            // play animation
            while (true)
            {
                // increment current sprite, if it is last sprite, reset to 0
                currentSprite += (currentSprite + 1 < sprites.Length) ? 1 : -currentSprite;
                // update sprite animation
                targetImage.sprite = sprites[currentSprite];
                // wait for frame duration before updating animation
                yield return new WaitForSeconds(durationBetweenFrames / 1000f);
            }
        }
    }
}
