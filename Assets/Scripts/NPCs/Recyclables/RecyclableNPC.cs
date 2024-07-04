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
        [SerializeField]
        private RecyclableType _recyclableType;
        public GameObject contaminant_prefab;
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

        public void OnZoneStart()
        {
            // todo activate the recyclable
        }
    }
}
