using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Level.Bins;

namespace Level
{
    [RequireComponent(typeof(LevelManager))]
    public class GameManager : MonoBehaviour
    {
        [SerializeField] string binTag = "Bin";

        LevelManager levelManager;
        RecyclingBin[][] bins;

        // Start is called before the first frame update
        void Start()
        {
            levelManager = LevelManager.Instance;
            bins = new RecyclingBin[levelManager.zones.Length][];
            for (int i = 0; i < bins.Length; i++)
            {
                bins[i] = levelManager.zones[i].GetComponentsInChildren<RecyclingBin>();
            }
            // Debug.Log(levelManager.zones[0].transform.childCount);
        }

        // Update is called once per frame
        void Update()
        {
            
        }
    }
}
