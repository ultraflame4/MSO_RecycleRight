using System;
using System.Collections;
using UnityEngine;
using UI.Animations;
using Random = UnityEngine.Random;

namespace UI.LevelSelection
{
    public class Hologram : MonoBehaviour
    {
        [Header("Transition Animation")]
        [SerializeField] float animationDuration = 1f;
        [SerializeField] float maxScale = 1f;

        [Header("Sprite Animation")]
        [SerializeField] UIAnimator anim;
        [SerializeField] Vector2 glitchCooldown;

        protected Coroutine coroutine_glitch_effect, coroutine_transition;
        public bool Active { get; protected set; } = false;

        /// <summary>
        /// Animate hologram when activating and deactivating by changing horizontal scale
        /// </summary>
        /// <param name="active">Active state. If true, animates opening. Else animates closing</param>
        /// <param name="callback">Callback after transition is complete</param>
        protected IEnumerator AnimateTransition(bool active, Action callback = null)
        {
            float timeElasped = 0f;
            Vector3 scale = transform.localScale;

            if (active) scale.x = 0f;

            while (timeElasped < animationDuration)
            {
                transform.localScale = scale;
                scale.x = maxScale * (active ? (timeElasped / animationDuration) : 
                    1f - (timeElasped / animationDuration));
                timeElasped += Time.deltaTime;
                yield return timeElasped;
            }

            if (active)
            {
                scale.x = maxScale;
                transform.localScale = scale;
            }
            else 
            {
                gameObject.SetActive(false);
            }

            coroutine_transition = null;
            callback?.Invoke();
        }

        /// <summary>
        /// Periodically play glitching animation
        /// </summary>
        /// <param name="glitch_callback">Callback when playing glitch animation</param>
        /// <param name="default_callback">Callback when returning to default state</param>
        protected IEnumerator Glitch(Action glitch_callback = null, Action default_callback = null)
        {
            anim?.Play("Default");
            yield return new WaitForSeconds(Random.Range(glitchCooldown.x, glitchCooldown.y));
            glitch_callback?.Invoke();
            anim?.Play("Glitch");
            yield return new WaitForSeconds(anim.currentAnimation.duration);
            default_callback?.Invoke();
            
            if (gameObject.activeInHierarchy)
                coroutine_glitch_effect = StartCoroutine(Glitch(glitch_callback, default_callback));
        }
    }
}
