using System;
using System.Collections;
using UnityEngine;
using Level.Bins;

namespace Level
{
    [RequireComponent(typeof(LevelManager))]
    public class GameManager : MonoBehaviour
    {
        [SerializeField] float zoneChangeDelay = 2.5f;
        [SerializeField] string binTag = "Bin";

        LevelManager levelManager;
        Coroutine coroutine;

        private static GameManager _instance;
        public static GameManager Instance 
        {
            get
            {
                if (_instance == null)
                {
                    throw new NullReferenceException("There is no game manager instance in the scene!");
                }
                return _instance;
            }
        }

        public RecyclingBin[][] Bins { get; private set; }

        private void Awake()
        {
            if (_instance == null)
                _instance = this;
            else
                Debug.LogWarning("There are multiple GameManager(s) in the scene! This is not allowed!");
        }

        // Start is called before the first frame update
        void Start()
        {
            levelManager = LevelManager.Instance;
            if (levelManager.zones == null) return;
            // get references to recycling bins, and disable other zones
            Bins = new RecyclingBin[levelManager.zones.Length][];
            for (int i = 0; i < levelManager.zones.Length; i++)
            {
                Bins[i] = levelManager.zones[i].GetComponentsInChildren<RecyclingBin>();
                if (i == 0) continue;
                SetZoneActive(false, i);
            }
        }

        // This is in late update because the check for zone completion should only be done after all the other logic has completed
        void LateUpdate()
        {
            if (levelManager.zones == null || 
                levelManager.zones[levelManager.current_zone_index].transform.childCount > 
                Bins[levelManager.current_zone_index].Length) 
                    return;
            
            // check for level completion
            if (levelManager.current_zone_index >= (levelManager.zones.Length - 1))
            {
                Debug.Log("Level Completed.");
                return;
            }

            if (coroutine != null) return;
            coroutine = StartCoroutine(DelayedZoneUpdate());
        }

        /// <summary>
        /// Set the active of the zone
        /// </summary>
        /// <param name="active">Whether to activate or deactivate zone</param>
        /// <param name="index">Index of zone</param>
        void SetZoneActive(bool active, int index)
        {
            if (levelManager.zones == null || levelManager.zones.Length <= index) return;
            foreach (Transform child in levelManager.zones[index].transform)
            {
                if (child.gameObject.CompareTag(binTag)) continue;
                child.gameObject.SetActive(active);
            }
        }

        IEnumerator DelayedZoneUpdate()
        {
            yield return new WaitForSeconds(zoneChangeDelay);
            SetZoneActive(true, levelManager.current_zone_index + 1);
            levelManager.MoveToZone(levelManager.current_zone_index + 1);
            coroutine = null;
        }
    }
}
