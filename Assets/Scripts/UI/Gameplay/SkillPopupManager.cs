using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Player;

namespace UI
{
    public class SkillPopupManager : MonoBehaviour
    {
        [SerializeField] GameObject container;
        [SerializeField] Image playerSprite;
        [SerializeField] float showDuration = 1.5f;

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
            container?.SetActive(true);

            while (timeElasped < showDuration)
            {
                // update player sprite
                playerSprite.sprite = player.Data.renderer.sprite;
                playerSprite.SetNativeSize();
                // increment time elasped
                timeElasped += Time.deltaTime;
                yield return timeElasped;

            }

            container?.SetActive(false);
        }
    }
}
