using System;
using UnityEngine;
using System.Linq;

namespace UI.Animations
{
    public class UIAnimator : MonoBehaviour
    {
        [SerializeField] UIAnimation[] animations;
        public UIAnimation currentAnimation { get; private set; }

        // Start is called before the first frame update
        void Start()
        {
            // play first animation
            animations[0].Play();
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
    }
}
