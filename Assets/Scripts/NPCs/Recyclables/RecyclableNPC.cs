using UnityEngine;
using Level;
using Level.Bins;
using NPC.States;
using NPC.Contaminant;
using NPC.Recyclable.States;
using Interfaces;

namespace NPC.Recyclable
{
    public class RecyclableNPC : FSMRecyclableNPC, IStunnable, ILevelEntity
    {

        #region States
        public RecyclableIdle state_Idle { get; private set; }
        public Flee state_Flee { get; private set; }
        public Stunned state_Stunned { get; private set; }
        #endregion

        #region Config
        
        [SerializeField, Tooltip("The recyclable type.")]
        private RecyclableType _recyclableType;
        [SerializeField, Tooltip("The contaminant version of this recyclable.")]
        private GameObject contaminant_prefab;
        public float sightRange = 3f;
        #endregion

        public override RecyclableType recyclableType => _recyclableType;

        public ContaminantNPC nearestContaminant { get; private set; } = null;

        private void Start()
        {
            state_Idle = new(this);
            state_Stunned = new(state_Idle, this, this);
            state_Flee = new(this);

            Initialize(state_Idle);
        }
        public void Contaminate(float damage)
        {
            // Debug.Log("Hit by Contaminant!");
            var contaminant = Instantiate(contaminant_prefab);
            contaminant.transform.position = transform.position;
            Destroy(gameObject);
        }

        public void Stun(float stun_duration)
        {
            state_Stunned.stun_timer = stun_duration;
            SwitchState(state_Stunned);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(transform.position, sightRange);
        }

        private void OnValidate() {
            if (recyclableType == RecyclableType.OTHERS){
                Debug.LogWarning("The 'OTHER' Recyclable type is meant for non-recyclables / contaminants! Using it on Recyclables will have unintended effects! You should probably use ContaminantNPC instead!");
            }
            if (contaminant_prefab == null){
                Debug.LogWarning("IMPORTANT! contaminant_prefab is a required field! When null, it will cause this recyclable to never spawn it's contaminated version");
            }
        }

        public void OnZoneStart()
        {
            // todo activate the recyclable
        }
    }
}
