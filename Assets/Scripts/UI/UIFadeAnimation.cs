using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UI
{
    public class UIFadeAnimation : MonoBehaviour
    {
        [SerializeField] float fadeDuration = .75f;
        [SerializeField] float fadeDelay = 1f;
        public float FadeDelay => fadeDelay;
        public float FadeDuration => fadeDuration;

        [SerializeField] FadeType fadeType = FadeType.FADE_IN;
        private enum FadeType
        {
            FADE_IN, 
            FADE_OUT, 
            BOTH
        }

        [SerializeField] bool startActive = false;
        
        List<UIItem> uiItems;
        private class UIItem
        {
            public GameObject gameObject { get; }
            public Color color { get; }
            public Color originalColor { get; }
            public Action<Color> UpdateColor { get; }

            public UIItem(GameObject gameObject, Color color, Action<Color> update_color_callback = null)
            {
                this.gameObject = gameObject;
                this.color = color;
                originalColor = new Color(color.r, color.g, color.b, color.a);
                UpdateColor = update_color_callback;
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            Image[] images = GetComponentsInChildren<Image>();
            TextMeshProUGUI[] textBoxes = GetComponentsInChildren<TextMeshProUGUI>();
            uiItems = new List<UIItem>();

            foreach (Image image in images)
            {
                uiItems.Add(new UIItem(image.gameObject, image.color, (Color color) => image.color = color));
                if (startActive) continue;
                image.gameObject.SetActive(false);
            }

            foreach (TextMeshProUGUI text in textBoxes)
            {
                uiItems.Add(new UIItem(text.gameObject, text.color, (Color color) => text.color = color));
                if (startActive) continue;
                text.gameObject.SetActive(false);
            }
        }

        public void SetActive(bool active)
        {
            StopAllCoroutines();

            foreach (UIItem item in uiItems)
            {
                StartCoroutine(Fade(item, active));
            }
        }

        IEnumerator Fade(UIItem item, bool active)
        {
            if ((fadeType == FadeType.FADE_IN && active) || 
                (fadeType == FadeType.FADE_OUT && !active) ||
                fadeType == FadeType.BOTH)
                    yield return new WaitForSeconds(fadeDelay);
            
            if (active) item.gameObject.SetActive(true);

            float timeElasped = 0f;
            Color color = item.color;
            color.a = active ? 0f : item.originalColor.a;

            while (timeElasped < fadeDuration)
            {
                timeElasped += Time.deltaTime;
                color.a = item.originalColor.a * (active ? timeElasped / fadeDuration : 1f - (timeElasped / fadeDuration));
                item.UpdateColor?.Invoke(color);
                yield return timeElasped;
            }

            color.a = active ? item.originalColor.a : 0f;
            item.UpdateColor?.Invoke(color);

            if (!active) item.gameObject.SetActive(false);
        }
    }
}
