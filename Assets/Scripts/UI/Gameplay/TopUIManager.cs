using UnityEngine;
using TMPro;
using Level;

namespace UI
{
    public class TopUIManager : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI topText;

        // Start is called before the first frame update
        void Start()
        {
            if (LevelManager.Instance == null)
            {
                Debug.LogWarning("Level Manager is null, level data could not be loaded! (TopUIManager.cs)");
                return;
            }

            if (topText == null) return;
            topText.text = LevelManager.Instance.levelInfo.Data.levelCode;
        }
    }
}

