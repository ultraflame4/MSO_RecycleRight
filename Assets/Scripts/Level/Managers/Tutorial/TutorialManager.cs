using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Level.Tutorial
{
    public class TutorialManager : MonoBehaviour
    {
        // inspector fields
        [SerializeField] ZoneTasks[] zoneTasks;

        // private fields
        LevelManager levelManager;
        TutorialTask currentTask;
        int currentTaskIndex;

        // Start is called before the first frame update
        void Start()
        {
            levelManager = LevelManager.Instance;
            // ensure level manager is not null
            if (levelManager == null)
            {
                Debug.LogError("Level Manager Instance is null (TutorialManager.cs)");
                return;
            }
            // log warning message if length of zone and tasks do not match
            if (zoneTasks.Length != levelManager.zones.Length) 
                Debug.LogWarning($"Length of tasks array ({zoneTasks.Length}) does " + 
                    $"not match total number of zones ({levelManager.zones.Length}).");
            // reset current task to 0
            currentTaskIndex = 0;
            // set current task
            SetTask();
        }

        void SetTask()
        {
            // set current task
            currentTask = zoneTasks[levelManager.current_zone_index].tasks[currentTaskIndex];
            // subscribe to task complete event
            currentTask.TaskCompleted += CompleteTask;
            // check if tutorial is completed
            if (currentTask.IsCompleted)
            {
                CompleteTask();
                return;
            }
            // activate tutorial
            currentTask.SetTutorialActive(true);
        }

        void CompleteTask()
        {
            Debug.Log("Task completed.");
            // unsubscribe from task complete event
            currentTask.TaskCompleted -= CompleteTask;
            // increment task index
            if (currentTaskIndex == (zoneTasks[levelManager.current_zone_index].tasks.Length - 1))
            {
                currentTaskIndex = 0;
                // check if is last zone
                if (levelManager.current_zone_index == (levelManager.zones.Length - 1))
                {
                    // handle win condition
                    Debug.Log("Tutorial stage completed.");
                    return;
                }
                // handle last task in zone
                levelManager.MoveToZone(levelManager.current_zone_index + 1);
            }
            // increment task in zone
            else
            {
                currentTaskIndex++;
            }
            // ensure there are tasks to set, then set task
            if (zoneTasks.Length > levelManager.current_zone_index) SetTask();
        }
    }
}
