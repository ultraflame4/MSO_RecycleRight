using System;
using System.Collections;
using UnityEngine;

namespace Level.Tutorial
{
    public abstract class TutorialTask : MonoBehaviour
    {
        [Tooltip("Minimum duration to keep tutorial active.")]
        [SerializeField] float minCompletionDuration = 5f;
        [Tooltip("UI elements to show for the tutorial.")]
        [SerializeField] GameObject[] UIElements;
        Coroutine coroutine;

        // public boolean properties
        public bool IsActive { get; private set; }
        public bool IsCompleted { get; private set; }

        // event to be called when task is marked as completed
        public event Action TaskCompleted;

        // Start is called before the first frame update
        protected void Start()
        {
            // reset coroutine
            coroutine = null;
            // reset properties
            IsActive = false;
            IsCompleted = false;
            // when game is starting, disable UI elements
            SetTutorialActive(false);
        }

        // Update is called once per frame
        protected void Update()
        {
            if (!IsActive || !CheckTaskCompletion() || coroutine != null) return;
            coroutine = StartCoroutine(CountDuration());
        }

        /// <summary>
        /// Toggle hiding and showing of tutorial UI elements.
        /// </summary>
        /// <param name="active">Whether to show (true) the tutorial or not (false)</param>
        public virtual void SetTutorialActive(bool active)
        {
            // set active of UI element objects
            foreach(GameObject obj in UIElements)
            {
                if (obj == null) continue;
                obj.SetActive(active);
            }
            // update is active property
            IsActive = active;
        }

        public abstract bool CheckTaskCompletion();

        IEnumerator CountDuration()
        {
            yield return new WaitForSeconds(minCompletionDuration);
            // reset tutorial
            SetTutorialActive(false);
            coroutine = null;
            // mark tutorial as completed
            IsCompleted = true;
            // invoke event
            TaskCompleted?.Invoke();
        }
    }
}
