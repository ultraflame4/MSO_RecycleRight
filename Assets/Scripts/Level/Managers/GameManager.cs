using UnityEngine;
using Level.Bins;

namespace Level
{
    [RequireComponent(typeof(LevelManager))]
    public class GameManager : MonoBehaviour
    {
        [SerializeField] string binTag = "Bin";

        LevelManager levelManager;
        public RecyclingBin[][] Bins { get; private set; }

        // Start is called before the first frame update
        void Start()
        {
            levelManager = LevelManager.Instance;
            // get references to recycling bins
            Bins = new RecyclingBin[levelManager.zones.Length][];
            for (int i = 0; i < levelManager.zones.Length; i++)
            {
                Bins[i] = levelManager.zones[i].GetComponentsInChildren<RecyclingBin>();
            }
        }

        // This is in late update because the check for zone completion should only be done after all the other logic has completed
        void LateUpdate()
        {
            if (levelManager.zones[levelManager.current_zone_index].transform.childCount > 
                Bins[levelManager.current_zone_index].Length) 
                    return;
            
            // check for level completion
            if (levelManager.current_zone_index >= (levelManager.zones.Length - 1))
            {
                Debug.Log("Level Completed.");
                return;
            }

            levelManager.MoveToZone(levelManager.current_zone_index + 1);
        }
    }
}
