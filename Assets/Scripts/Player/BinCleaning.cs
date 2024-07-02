using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Entity.Data;
using Level.Bins;
using Level;

namespace Player.BinCleaning
{
    public class BinCleaning : MonoBehaviour
    {
        #region Inspector Fields
        [Tooltip("Duration to clean bin")]
        public float binCleanDuration = 15f;
        [Tooltip("Maximum distance from bin to be able to clean it")]
        public float binCleanRange = 2f;
        [Tooltip("Offset character from bin when cleaning it")]
        public Vector3 binCleanOffset;
        [SerializeField, Tooltip("Layer mask to detect bin")]
        private LayerMask binMask;
        #endregion

        #region Private Variables
        PlayerController controller;
        PlayerCharacter currentCharacterData;
        Coroutine cleaning_bin_coroutine;
        #endregion

        #region MonoBehaviour Callbacks
        // Start is called before the first frame update
        void Start()
        {
            // get reference to player controller
            controller = GetComponentInParent<PlayerController>();
            // get reference to the character data of this character
            currentCharacterData = GetComponent<PlayerCharacter>();
            // subscribe to character change event
            controller.CharacterManager.CharacterChanged += OnCharacterChange;
            // subscribe to zone change event
            controller.LevelManager.ZoneChanged += OnZoneChange;
        }

        // Update is called once per frame
        void Update()
        {
            
        }
        #endregion

        PlayerCharacter FindBinCleaner()
        {
            // check if there are more than 1 remaining characters
            if (controller.CharacterManager.character_instances.Length <= 1 &&
                controller.CharacterManager.character_instances.Length > 0)
                    return controller.Data;
            // return current character if there 
            return currentCharacterData;
        }

        void CleanBin(Transform cleaner, Transform bin)
        {

        }

        IEnumerator CleaningBin()
        {
            float timeElasped = 0f;

            while (timeElasped < binCleanDuration)
            {
                // increment time elapsed
                timeElasped += Time.deltaTime;
                yield return timeElasped;
            }
        }

        #region Event Listeners
        void OnCharacterChange(PlayerCharacter data)
        {
            // check if character that was switched to is the current character, if not do not run
            if (data != currentCharacterData) return;
            // check if near a bin
            Collider2D[] hits = Physics2D.OverlapCircleAll(controller.transform.position, binCleanRange, binMask);
            // filter out uncontaminated bins
            hits = hits
                .Select(x => x.GetComponent<RecyclingBin>())
                .Where(x => x != null && x.binState == BinState.CONTAMINATED)
                .Select(x => x.GetComponent<Collider2D>())
                .Where(x => x != null)
                .ToArray();
            // check if anything still remains after the filter
            if (hits.Length <= 0) return;
        }

        void OnZoneChange(LevelManager manager, LevelZone zone)
        {
            // stop cleaning proccess when zone changes
            if (cleaning_bin_coroutine == null) return;
            StopCoroutine(cleaning_bin_coroutine);
            cleaning_bin_coroutine = null;
        }
        #endregion

        #region Gizmos
        void OnDrawGizmosSelected()
        {
            // ensure player controller is not null
            if (controller == null) return;
            // show bin interaction range
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(controller.transform.position, binCleanRange);
        }
        #endregion
    }   
}
