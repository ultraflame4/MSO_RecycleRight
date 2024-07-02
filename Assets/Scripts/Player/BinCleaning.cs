using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player.BinCleaning
{
    public class BinCleaning : MonoBehaviour
    {
        [Tooltip("Duration to clean bin")]
        public float binCleanDuration = 15f;
        [Tooltip("Maximum distance from bin to be able to clean it")]
        public float binCleanRange = 2f;
        [Tooltip("Offset character from bin when cleaning it")]
        public Vector3 binCleanOffset;

        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            
        }
    }   
}
