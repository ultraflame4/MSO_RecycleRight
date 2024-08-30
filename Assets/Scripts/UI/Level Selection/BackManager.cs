using UnityEngine;
using UnityEngine.UI;

namespace UI.LevelSelection
{
    public class BackManager : MonoBehaviour
    {
        [SerializeField] KeyCode backKey = KeyCode.Escape;

        [Header("Buttons")]
        [SerializeField] Button btnMapBack;
        [SerializeField] Button btnLevelDetailsBack;

        [Header("Pages")]
        [SerializeField] GameObject levelDetailsMenu;
        [SerializeField] GameObject characterSelectMenu;

        // Update is called once per frame
        void Update()
        {
            // check if back key has been pressed, if so, trigger corresponding button OnClick
            if (!Input.GetKeyDown(backKey)) return;
            // do not check for back button press if character select is open
            if (characterSelectMenu.activeInHierarchy) return;

            // check which OnClick even can be invoked
            if (levelDetailsMenu.activeInHierarchy)
                btnLevelDetailsBack.onClick.Invoke();
            else
                btnMapBack.onClick.Invoke();
        }
    }
}
