using System;
using UnityEngine;
using System.Linq;

namespace UI.Animations
{
    public class UIAnimator : MonoBehaviour
    {
        [SerializeField] bool playOnAwake = true;
        [SerializeField] UIAnimation[] animations;
        public UIAnimation currentAnimation { get; private set; }

        void Awake()
        {
            // check if need to play animation on awake
            if (!playOnAwake) return;
            // play first animation
            Play(animations[0].Name);
        }

        /// <summary>
        /// Play an animation in the animations list
        /// </summary>
        /// <param name="name">Name of animation to play</param>
        public void Play(string name)
        {
            // if animations list do not contain animation with the name
            // log it and do not run
            if (!animations.Select(x => x.Name).Contains(name))
            {
                Debug.LogWarning("Animation of name '" + name + "' could not be found. (UIAnimator.cs)");
                return;
            }
            // stop other animations
            foreach (UIAnimation animation in animations.Where(x => x.Name != name))
            {
                animation.Stop();
            }
            // play animation with the name
            currentAnimation = animations.Where(x => x.Name == name).ToArray()[0];
            currentAnimation.Play();
        }

        /// <summary>
        /// Stop playing current animation and reset sprite
        /// </summary>
        public void Stop()
        {
            if (currentAnimation == null) return;
            currentAnimation.Stop();
        }

        /// <summary>
        /// Pause current animation
        /// </summary>
        public void Pause()
        {
            if (currentAnimation == null) return;
            currentAnimation.Stop(false);
        }

        /// <summary>
        /// Continue current animation without resetting progress
        /// </summary>
        public void Continue()
        {
            if (currentAnimation == null) return;
            currentAnimation.Play(false);
        }
    }
}
