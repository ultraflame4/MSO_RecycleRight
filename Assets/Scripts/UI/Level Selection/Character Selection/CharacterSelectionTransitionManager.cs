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


        /// <summary>
        /// Transition to black screen.
        /// </summary>
        /// <param name="flip"></param>
        /// <param name="invert">When inverted, transitions from black to normal.</param>
        /// <returns></returns>
        public IEnumerator HalfTransition(bool flip, bool invert)
        {
            float timeElasped = 0f;
            float duration = transitionDuration / 2f;
            Vector3 scale = transform.localScale;

            scale.x = flip ? 1f : -1f;
            transform.localScale = scale;
            image.materialForRendering.SetFloat("_CutOff", 0f);

            while (timeElasped < (transitionDuration / 2f))
            {
                timeElasped += Time.deltaTime;
                float percent = timeElasped /  duration;
                image.materialForRendering.SetFloat("_CutOff", invert ? 1 - percent : percent);
                yield return null;
            }


        }

    }
}
