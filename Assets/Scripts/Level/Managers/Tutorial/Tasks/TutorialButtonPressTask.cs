using UnityEngine;

namespace Level.Tutorial
{
    public class TutorialButtonPressTask : TutorialTaskWithInfoBox
    {
        [Header("Button Press")]
        [SerializeField] KeyCode[] keys;
        [SerializeField] int[] mouseButtons;

        public override bool CheckTaskCompletion()
        {
            bool taskComplete = false;

            // check key press
            foreach (KeyCode key in keys)
            {
                if (!Input.GetKeyDown(key)) continue;
                taskComplete = true;
                break;
            }

            // check mouse button press
            foreach (int mouseButton in mouseButtons)
            {
                if (taskComplete) break;
                if (!Input.GetMouseButtonDown(mouseButton)) continue;
                taskComplete = true;
            }

            if (taskComplete) box.IncrementCount();

            return taskComplete;
        }
    }
}
