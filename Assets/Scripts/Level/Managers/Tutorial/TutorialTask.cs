using System;
using System.Collections;
using UnityEngine;
using NPC;
using System.Xml.Serialization;

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
        [SerializeField, Tooltip("Minimum duration to keep tutorial active.")]
        float minCompletionDuration = 5f;

        [SerializeField, Tooltip("UI elements to show for the tutorial.")]
        GameObject[] UIElements;

        [Header("Reset")]
        [SerializeField, Tooltip("List of recyclables to be reset when the player needs to retry the tutorial. ")]
        protected Recyclable[] recyclables;
        [SerializeField, Tooltip("List of game objects to enable / disable.")]
        protected GameObject[] controlledEntities;

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
            SetActiveUIElements(false);

            // Store original recyclable positions and parents
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
            // Don't run completion check if alr compelted OR task not active.
            if (!IsActive || IsCompleted) return;
            var complete = CheckTaskCompletion();
            if (complete){
                EndTask();
            }
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


        private void SetActiveUIElements(bool active){
            foreach(GameObject obj in UIElements)
            {
                if (obj == null) continue;
                obj.SetActive(active);
            }    
        }

        public void ResetTask()
        {
            foreach (var item in controlledEntities)
            {
                item.SetActive(IsActive);
            }   
        }

        /// <summary>
        /// Starts the task
        /// </summary>
        public virtual void StartTask(){
            IsActive = true;
            Debug.Log($"Starting task: {gameObject.name}");
            ResetTask();
            SetActiveUIElements(true);
        }

        /// <summary>
        /// Ends the task
        /// </summary>
        public virtual void EndTask(){
            IsActive = true;
            IsCompleted = true;
            SetActiveUIElements(false);
            Debug.Log($"Ended task: {gameObject.name}");
        }


        /// <summary>
        /// Called every frame when task is active to check if task is complete.
        /// </summary>
        /// <returns>True if completed, otherwise false</returns>
        public abstract bool CheckTaskCompletion();



    }
}
