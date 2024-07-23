using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UI.LevelSelection
{
    public class DetailedLevelMenu : MonoBehaviour
    {
        [SerializeField] float fadeDelay = 1f;
        [SerializeField] float fadeDuration = .75f;
        Image[] images;

        // Start is called before the first frame update
        void Start()
        {
            images = GetComponentsInChildren<Image>();

            foreach (Image image in images)
            {
                Color color = image.color;
                color.a = 0f;
                image.color = color;

                image.gameObject.SetActive(false);
            }
        }

        public void SetActive(bool active)
        {
            StopAllCoroutines();

            foreach (Image image in images)
            {
                StartCoroutine(Fade(image, active));
            }
        }

        IEnumerator Fade(Image image, bool active)
        {
            if (active) 
            {
                yield return new WaitForSeconds(fadeDelay);
                image.gameObject.SetActive(true);
            }

            float timeElasped = 0f;
            Color color = image.color;
            color.a = active ? 0f : 1f;

            while (timeElasped < fadeDuration)
            {
                timeElasped += Time.deltaTime;
                color.a = active ? timeElasped / fadeDuration : 1f - (timeElasped / fadeDuration);
                image.color = color;
                yield return timeElasped;
            }

            color.a = active ? 1f : 0f;
            image.color = color;

            if (!active) image.gameObject.SetActive(false);
        }
    }
}
