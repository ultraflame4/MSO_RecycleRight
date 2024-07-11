using UnityEngine;
using TMPro;

namespace UI
{
    public class InformationBox : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI informationText;
        [SerializeField] TextMeshProUGUI countText;
        [SerializeField] int maxCount = 0;
        int currentCount;

        // Start is called before the first frame update
        void Start()
        {
            ResetCount();
        }

        /// <summary>
        /// Change the information in the information box. 
        /// </summary>
        /// <param name="text">Text description</param>
        /// <param name="count">Number of times players have to perform instructed action</param>
        public void SetInformation(string text, int count = 0)
        {
            informationText.text = text;
            maxCount = count;
            UpdateCountUI();
        }

        /// <summary>
        /// Increment action count by 1
        /// </summary>
        public void IncrementCount()
        {
            currentCount++;
            UpdateCountUI();
        }

        /// <summary>
        /// Reset action count to 0
        /// </summary>
        public void ResetCount()
        {
            currentCount = 0;
            UpdateCountUI();
        }

        /// <summary>
        /// Update action count in UI
        /// </summary>
        public void UpdateCountUI()
        {
            if (maxCount == 0)
            {
                countText.text = "";
                countText.gameObject.SetActive(false);
                return;
            }
            // ensure text is active
            if (!countText.gameObject.activeSelf) 
                countText.gameObject.SetActive(true);
            // set text
            countText.text = $"{currentCount}/{maxCount}";
        }
    }
}
