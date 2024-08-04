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

        protected IEnumerator AnimateTransition(bool active, Action active_callback = null, Action inactive_callback = null)
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

            coroutine_transition = null;

            if (active)
            {
                scale.x = maxScale;
                transform.localScale = scale;
                active_callback?.Invoke();
            }
            else 
            {
                gameObject.SetActive(false);
                inactive_callback?.Invoke();
            }
        }

        protected IEnumerator Glitch(Action glitch_callback = null, Action default_callback = null)
        {
            anim?.Play("Default");
            yield return new WaitForSeconds(Random.Range(glitchCooldown.x, glitchCooldown.y));
            glitch_callback?.Invoke();
            anim?.Play("Glitch");
            yield return new WaitForSeconds(anim.currentAnimation.duration);
            default_callback?.Invoke();
            coroutine_glitch_effect = StartCoroutine(Glitch(glitch_callback, default_callback));
        }
    }
}
