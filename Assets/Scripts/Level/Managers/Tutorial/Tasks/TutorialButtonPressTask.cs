using System;
using UnityEngine;

namespace Level.Tutorial
{
    public class TutorialButtonPressTask : TutorialTaskWithInfoBox
    {
        [Header("Button Press")]
        [SerializeField] KeyCode[] keys;
        [SerializeField] int[] mouseButtons;
        int currentCount;

        /// <summary>
        /// It is called whenever a button press is detected. 
        /// </summary>
        public event Action<int> ButtonPressed;

        new protected void Start()
        {
            base.Start();
            currentCount = 0;
        }

        public override bool CheckTaskCompletion()
        {
            bool taskComplete = false;

            // check key press
            taskComplete = CheckButtonPress(keys, (KeyCode key) => Input.GetKeyDown(key));
            // check mouse button press
            if (!taskComplete)
                taskComplete = CheckButtonPress(mouseButtons, (int mouseButton) => Input.GetMouseButtonDown(mouseButton));

            if (taskComplete) 
            {
                currentCount++;
                box.IncrementCount();
                ButtonPressed?.Invoke(currentCount);
            }

            return currentCount >= count || (count <= 0 && taskComplete);
        }

        protected delegate bool Condition<T>(T element);
        protected bool CheckButtonPress<T>(T[] array, Condition<T> condition)
        {
            if (array == null || array.Length <= 0) return false;

            foreach (var element in array)
            {
                if (!condition.Invoke(element)) continue;
                return true;
            }

            return false;
        }
    }
}
