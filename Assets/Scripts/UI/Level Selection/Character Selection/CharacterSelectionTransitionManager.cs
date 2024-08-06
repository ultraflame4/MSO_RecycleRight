using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UI.LevelSelection.CharacterSelection
{
    public class CharacterSelectionTransitionManager : MonoBehaviour
    {
        [SerializeField] float transitionDuration = 1f;
        [SerializeField] Image image;
        Coroutine coroutine_transition;

        public event Action MakeTransition;

        /// <summary>
        /// Play transition animation
        /// </summary>
        /// <param name="forward_direction">Boolean controlling if the animation is moving in the forward direction</param>
        public void PlayTransition(bool forward_direction)
        {
            if (image == null) return;
            if (coroutine_transition != null) StopCoroutine(coroutine_transition);
            StartCoroutine(Transition(forward_direction));
        }

        IEnumerator Transition(bool forward_direction)
        {
            float timeElasped = 0f;
            Vector3 scale = transform.localScale;

            scale.x = forward_direction ? 1f : -1f;
            transform.localScale = scale;
            image.materialForRendering.SetFloat("_CutOff", 0f);

            while (timeElasped < (transitionDuration / 2f))
            {
                timeElasped += Time.deltaTime;
                image.materialForRendering.SetFloat("_CutOff",  timeElasped / (transitionDuration / 2f));
                yield return timeElasped;
            }

            scale.x = forward_direction ? -1f : 1f;
            transform.localScale = scale;
            image.materialForRendering.SetFloat("_CutOff", 1f);
            MakeTransition?.Invoke();

            while (timeElasped < transitionDuration)
            {
                timeElasped += Time.deltaTime;
                image.materialForRendering.SetFloat("_CutOff",  1f - ((timeElasped - (transitionDuration / 2f)) / (transitionDuration / 2f)));
                yield return timeElasped;
            }

            image.materialForRendering.SetFloat("_CutOff", 0f);
            coroutine_transition = null;
        }
    }
}
