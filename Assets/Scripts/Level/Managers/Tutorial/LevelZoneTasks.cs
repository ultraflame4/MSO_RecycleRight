using UnityEngine;

namespace Level.Tutorial
{
    public class LevelZoneTasks : MonoBehaviour, ILevelEntity
    {
        [field: SerializeField]
        public TutorialTask[] tasks { get; private set; }
        public void OnZoneStart()
        {

        }
        
        public void OnZoneEnd()
        {

        }

    }
}
