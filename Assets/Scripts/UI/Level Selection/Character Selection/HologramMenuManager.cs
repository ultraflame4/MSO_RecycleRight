using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI.Animations;

namespace UI.LevelSelection.CharacterSelection
{
    public class HologramMenuManager : MonoBehaviour
    {
        [Header("Sprite Animation")]
        [SerializeField] UIAnimator anim;
        [SerializeField] Vector2 glitchCooldown;
        Coroutine coroutine_glitch_effect;

        // Start is called before the first frame update
        void Start()
        {
            coroutine_glitch_effect = StartCoroutine(Glitch());
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        /// <summary>
        /// Set the active state of this game object
        /// </summary>
        /// <param name="active">Active state to set to</param>
        public void SetActive(bool active)
        {

        }

        IEnumerator Glitch()
        {
            yield return new WaitForSeconds(Random.Range(glitchCooldown.x, glitchCooldown.y));
            anim.Play("Glitch");
            yield return new WaitForSeconds(anim.currentAnimation.duration);
            anim.Play("Default");
            coroutine_glitch_effect = StartCoroutine(Glitch());
        }
    }
}
