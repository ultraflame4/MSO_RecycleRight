using UnityEngine;
using TMPro;

namespace UI.LevelSelection
{
    public class LevelDetailsMenu : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI levelNameText, levelDescText;
        [field: SerializeField] public UIFadeAnimation anim { get; private set; }

        public void SetText(string levelName, string levelDesc)
        {
            if (levelNameText != null) levelNameText.text = levelName;
            if (levelDescText != null) levelDescText.text = levelDesc;
        }
    }
}
