using System.Collections;
using UnityEngine;
using UI.Animations;

namespace UI
{
    public class ToggleSwitch : MonoBehaviour
    {
        [SerializeField] bool defaultActive = false;
        [SerializeField] UIAnimator anim;
        Coroutine coroutine_transition;

        public bool activated { get; private set; } = false;

        // Start is called before the first frame update
        void Start()
        {
            activated = defaultActive;
            SetAnimation();
        }

        /// <summary>
        /// Toggle the switch activation
        /// </summary>
        public void Toggle()
        {
            activated = !activated;
            SetAnimation();
        }

        void SetAnimation()
        {
            if (anim == null) return;
            if (coroutine_transition != null) StopCoroutine(coroutine_transition);
            coroutine_transition = StartCoroutine(PlayTransitionAnimation());
        }

        IEnumerator PlayTransitionAnimation()
        {
            anim.Play(activated ? "Transition On" : "Transition Off");
            yield return new WaitForSeconds(anim.currentAnimation.duration);
            anim.Play(activated ? "On" : "Off");
        }
    }
}

