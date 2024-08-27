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
        [SerializeField, Range(0f, 1f)] float cancelPopupWindow = 0.25f;

        PlayerController player => PlayerController.Instance;
        Coroutine coroutine_show, coroutine_hide;

        // Update is called once per frame
        void Update()
        {
            if (player == null || player.Data == null || 
                player.Data.renderer == null) 
                    return;

            if (player.currentState != player.SkillState) 
            {
                coroutine_show = null;
                coroutine_hide = null;
                return;
            }

            if (coroutine_show != null) return;
            coroutine_show = StartCoroutine(ShowPopup());
        }

        void CheckCancelPopup()
        {
            if (!Input.anyKeyDown) return;
            if (coroutine_show == null || coroutine_hide != null) return;
            StopCoroutine(coroutine_show);
            coroutine_hide = StartCoroutine(HidePopup());
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
                
                // check if popup can be canceled
                if (timeElasped >= showDuration * cancelPopupWindow) 
                    CheckCancelPopup();
                
                // increment time elasped
                timeElasped += Time.deltaTime;
                yield return timeElasped;
            }

            coroutine_hide = StartCoroutine(HidePopup());
        }

        IEnumerator HidePopup()
        {
            float timeElasped = 0f;

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
