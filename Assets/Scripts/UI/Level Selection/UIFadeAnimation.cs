using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UI
{
    public class UIFadeAnimation : MonoBehaviour
    {
        [SerializeField] float fadeDelay = 1f;
        [SerializeField] float fadeDuration = .75f;
        [SerializeField] bool startActive = false;
        Image[] images;
        TextMeshProUGUI[] textBoxes;


        // Start is called before the first frame update
        void Start()
        {
            images = GetComponentsInChildren<Image>();
            textBoxes = GetComponentsInChildren<TextMeshProUGUI>();

            if (startActive) return;

            foreach (Image image in images)
            {
                image.gameObject.SetActive(false);
            }

            foreach (TextMeshProUGUI text in textBoxes)
            {
                text.gameObject.SetActive(false);
            }
        }

        public void SetActive(bool active)
        {
            StopAllCoroutines();

            foreach (Image image in images)
            {
                StartCoroutine(Fade(active, image.gameObject, image.color, (Color color, bool currentActive) => 
                    {
                        image.color = color;
                        image.gameObject.SetActive(currentActive);
                    }
                ));
            }

            foreach (TextMeshProUGUI text in textBoxes)
            {
                StartCoroutine(Fade(active, text.gameObject, text.color, (Color color, bool currentActive) => 
                    {
                        text.color = color;
                        text.gameObject.SetActive(currentActive);
                    }
                ));
            }
        }

        IEnumerator Fade(bool active, GameObject gameObject, Color color, Action<Color, bool> update_callback = null)
        {
            if (active) 
            {
                yield return new WaitForSeconds(fadeDelay);
                gameObject.SetActive(true);
            }

            float timeElasped = 0f;
            color.a = active ? 0f : 1f;

            while (timeElasped < fadeDuration)
            {
                timeElasped += Time.deltaTime;
                color.a = active ? timeElasped / fadeDuration : 1f - (timeElasped / fadeDuration);
                update_callback?.Invoke(color, gameObject.activeSelf);
                yield return timeElasped;
            }

            color.a = active ? 1f : 0f;

            if (!active) gameObject.SetActive(false);
        }
    }
}
