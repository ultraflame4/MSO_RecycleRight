using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Player;

namespace UI
{
    public class SkillPopupManager : MonoBehaviour
    {
        [SerializeField] CanvasGroup container;
        [SerializeField] Image playerSprite;
        [SerializeField] float showDuration = 1.5f;
        [SerializeField] float fadeOutDuration = 0.5f;

        PlayerController player => PlayerController.Instance;
        Coroutine coroutine;

        // Update is called once per frame
        void Update()
        {
            if (player == null || player.Data == null || 
                player.Data.renderer == null) 
                    return;

            if (player.currentState != player.SkillState) 
            {
                coroutine = null;
                return;
            }

            if (coroutine != null) return;
            coroutine = StartCoroutine(ShowPopup());
        }

        IEnumerator ShowPopup()
        {
            float timeElasped = 0f;
            container.gameObject.SetActive(true);
            container.alpha = 1f;

            while (timeElasped < showDuration)
            {
                // update player sprite
                playerSprite.sprite = player.Data.renderer.sprite;
                playerSprite.SetNativeSize();
                // increment time elasped
                timeElasped += Time.deltaTime;
                yield return timeElasped;
            }

            timeElasped = 0f;

            while (timeElasped < fadeOutDuration)
            {
                // fade out container
                container.alpha = Mathf.Clamp01(1f - (timeElasped / fadeOutDuration));
                // increment time elasped
                timeElasped += Time.deltaTime;
                yield return null;
            }

            container.alpha = 0f;
            container.gameObject.SetActive(false);
        }
    }
}
