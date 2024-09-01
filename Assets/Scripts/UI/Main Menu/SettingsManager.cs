using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UI.MainMenu
{
    public class SettingsManager : MonoBehaviour
    {
        [Header("Volume")]
        [SerializeField] Slider volumeSlider;
        [SerializeField] TextMeshProUGUI volumeText;

        [Header("Back")]
        [SerializeField] KeyCode backKey = KeyCode.Escape;
        [SerializeField] Button backButton;

        void Update()
        {
            // check if back key is pressed, if so, trigger button OnClick
            if (!Input.GetKeyDown(backKey)) return;
            backButton.onClick.Invoke();
        }

        void OnEnable()
        {
            // update volume slider to actual volume
            if (SoundManager.Instance == null) return;
            volumeSlider.value = SoundManager.Instance.volume;
            volumeText.text = Mathf.RoundToInt((volumeSlider.value * 100f)).ToString();
        }

        /// <summary>
        /// Update volume
        /// </summary>
        public void UpdateVolume()
        {
            if (SoundManager.Instance == null) return;
            SoundManager.Instance.volume = volumeSlider.value;
            volumeText.text = Mathf.RoundToInt((volumeSlider.value * 100f)).ToString();
        }
    }
}
