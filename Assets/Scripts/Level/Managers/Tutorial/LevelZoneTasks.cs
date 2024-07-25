using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Level.Tutorial
{
    public class LevelZoneTasks : MonoBehaviour, ILevelEntity
    {
        [field: SerializeField]
        public TutorialTask[] tasks { get; private set; }
        List<TutorialTask> pendingTasks = new List<TutorialTask>();

        private Coroutine iterTask = null;
        private LevelZone zone;
        private void Start()
        {
            zone = GetComponentInParent<LevelZone>();
            zone.skipCompletionCheck = true;
        }

        public void OnZoneStart()
        {
            pendingTasks.AddRange(tasks);

            if (iterTask != null)
            {
                StopCoroutine(iterTask);
            }
            iterTask = StartCoroutine(IterateTasks_Coroutine());
        }

        public void OnZoneEnd()
        {
            if (iterTask != null)
            {
                StopCoroutine(iterTask);
            }
        }

        private void OnAllTasksFinished()
        {
            zone.ForceComplete();
        }

        private IEnumerator IterateTasks_Coroutine()
        {
            foreach (var task in pendingTasks)
            {
                task.StartTask();
                while (!task.IsCompleted)
                {
                    yield return null;
                }
            }
            OnAllTasksFinished();
        }

    }
}
