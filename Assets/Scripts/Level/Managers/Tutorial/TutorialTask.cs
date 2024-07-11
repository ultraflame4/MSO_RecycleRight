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
        [SerializeField] GameObject[] UIelements;
        Coroutine coroutine;

        // public boolean properties
        public bool IsActive { get; private set; }
        public bool IsCompleted { get; private set; }

        // event to be called when task is marked as completed
        public event Action TaskCompleted;

        // Start is called before the first frame update
        void Start()
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
        void Update()
        {
            if (!IsActive || !CheckTaskCompletion() || coroutine != null) return;
            coroutine = StartCoroutine(CountDuration());
        }

        public void SetTutorialActive(bool active)
        {
            // set active of UI element objects
            foreach(GameObject obj in UIelements)
            {
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
