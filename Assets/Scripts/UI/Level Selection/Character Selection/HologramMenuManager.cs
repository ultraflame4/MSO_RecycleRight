using System.Collections;
using UnityEngine;
using UI.Animations;

namespace UI.LevelSelection.CharacterSelection
{
    public class HologramMenuManager : MonoBehaviour
    {
        [Header("Menu Pages")]
        [SerializeField] CharacterSelectProfileManager characterList;

        [Header("Transition Animation")]
        [SerializeField] float animationDuration = 1f;
        [SerializeField] float maxScale = 1f;

        [Header("Sprite Animation")]
        [SerializeField] UIAnimator anim;
        [SerializeField] Vector2 glitchCooldown;

        Coroutine coroutine_glitch_effect, coroutine_transition;
        
        public CharacterSelectProfileManager CharacterList => characterList;
        public bool Active { get; private set; } = false;

        // Start is called before the first frame update
        void Start()
        {
            coroutine_glitch_effect = StartCoroutine(Glitch());
        }

        #region Active Management
        /// <summary>
        /// Set the active state of this game object
        /// </summary>
        /// <param name="active">Active state to set to</param>
        public void SetActive(bool active)
        {
            Active = active;

            if (active) 
            {
                gameObject.SetActive(true);
                characterList?.LoadCharacters();
            }

            if (coroutine_transition != null) StopCoroutine(coroutine_transition);
            coroutine_transition = StartCoroutine(AnimateTransition(active));
        }

        IEnumerator AnimateTransition(bool active)
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
                if (coroutine_glitch_effect != null) StopCoroutine(coroutine_glitch_effect);
                coroutine_glitch_effect = StartCoroutine(Glitch());
            }
            else 
            {
                gameObject.SetActive(false);
            }
        }
        #endregion
        
        #region Sprite Animation
        IEnumerator Glitch()
        {
            yield return new WaitForSeconds(Random.Range(glitchCooldown.x, glitchCooldown.y));
            anim.Play("Glitch");
            yield return new WaitForSeconds(anim.currentAnimation.duration);
            anim.Play("Default");
            coroutine_glitch_effect = StartCoroutine(Glitch());
        }
        #endregion
    }
}
