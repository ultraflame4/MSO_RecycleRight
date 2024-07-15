using System;
using System.Collections;
using UnityEngine;
using NPC;

namespace Level.Tutorial
{
    [Serializable]
    public struct Recyclable
    {
        // inspector fields
        public GameObject gameObject;
        public GameObject prefab;
        // hidden fields
        [HideInInspector] public Vector3 originalPosition;
        [HideInInspector] public Transform originalParent;
    }
    
    public abstract class TutorialTask : MonoBehaviour
    {
        [Header("Task")]
        [Tooltip("Minimum duration to keep tutorial active.")]
        [SerializeField] float minCompletionDuration = 5f;
        [Tooltip("UI elements to show for the tutorial.")]
        [SerializeField] GameObject[] UIElements;

        [Header("Reset")]
        [Tooltip("List of recyclables to be reset when the player needs to retry the tutorial. ")]
        [SerializeField] protected Recyclable[] recyclables;

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
            // cache original recyclable positions and parents
            if (recyclables == null) return;
            for (int i = 0; i < recyclables.Length; i++)
            {
                recyclables[i].originalPosition = recyclables[i].gameObject.transform.position;
                recyclables[i].originalParent = recyclables[i].gameObject.transform.parent;
            }
        }

        // Update is called once per frame
        protected void Update()
        {
            if (!IsActive || !CheckTaskCompletion() || coroutine != null) return;
            coroutine = StartCoroutine(CountDuration());
        }

        /// <summary>
        /// This method is to be called to reset recyclables for the task to their original positions
        /// </summary>
        /// <param name="disableMovement">Optional boolean whether or not to disable the movement of resetted recyclables</param>
        protected void ResetRecyclables(bool disableMovement = true)
        {
            for (int i = 0; i < recyclables.Length; i++)
            {
                if (recyclables[i].gameObject != null) Destroy(recyclables[i].gameObject);
                recyclables[i].gameObject = Instantiate(
                    recyclables[i].prefab, 
                    recyclables[i].originalPosition, 
                    Quaternion.identity, 
                    recyclables[i].originalParent
                );
                if (!disableMovement) continue;
                recyclables[i].gameObject.GetComponent<Navigation>().enabled = false;
            }
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
