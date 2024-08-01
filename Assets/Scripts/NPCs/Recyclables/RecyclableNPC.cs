using UnityEngine;
using Level;
using Level.Bins;
using NPC.States;
using NPC.Contaminant;
using NPC.Recyclable.States;
using Interfaces;
using System;

namespace NPC.Recyclable
{
    public class RecyclableNPC : FSMRecyclableNPC, IStunnable, IDamagable
    {

        #region States
        public RecyclableIdle state_Idle { get; private set; }
        public Flee state_Flee { get; private set; }
        public Stunned state_Stunned { get; private set; }
        #endregion

        #region Config

        [Header("Config"), SerializeField, Tooltip("The recyclable type.")]
        private RecyclableType _recyclableType;
        [SerializeField, Tooltip("The contaminant version of this recyclable.")]
        private GameObject contaminant_prefab;
        public float sightRange = 3f;
        [Tooltip("The npc data to configure this npc. Please note that this will override the above settings (on awake).")]
        public TrashNpcSO npcData;
        #endregion

        public override RecyclableType recyclableType => _recyclableType;

        public ContaminantNPC nearestContaminant { get; private set; } = null;

        // The internal 'cleanliness' meter so that recyclables don't immediately get contaminated. When above 0, still considered clean.
        private int secret_cleanliness = 3;

        /// <summary>
        /// Recyclables do not cause infestation and hence this is always false.
        /// </summary>
        public override bool cause_infestation => false;
        private bool spawned_contaminant = false;

        public void LoadConfig()
        {
            if (npcData == null) return;

            if (!npcData.containsRecyclable)
            {
                throw new ArgumentException("This RecyclableNPC is not configured as a Recyclable! Please change trashNPCType to Recyclable or use ContaminantNPC instead!");
            }
            sightRange = npcData.common.sightRange;
            _recyclableType = npcData.recyclableConfig.recyclableType;
            contaminant_prefab = npcData.contaminantConfig.contaminantPrefab;
        }
        private void Awake()
        {
            LoadConfig();
        }

        protected virtual void Start()
        {
            state_Idle = new(this);
            state_Stunned = new(state_Idle, this, this);
            state_Flee = new(this);

            Initialize(state_Idle);
        }
        public virtual void Contaminate(float damage)
        {
            if (secret_cleanliness > 0)
            {
                secret_cleanliness--;
                return;
            }
            SpawnRecyclable();
        }

        public void Stun(float stun_duration)
        {
            state_Stunned.stun_timer = stun_duration;
            SwitchState(state_Stunned);
        }

        public override void SpawnRecyclable()
        {
            base.SpawnRecyclable();
            if (spawned_contaminant) return;
            spawned_contaminant = true;
            var contaminant = Instantiate(contaminant_prefab);
            contaminant.transform.position = transform.position;
            Destroy(gameObject);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(transform.position, sightRange);
        }

        private void OnValidate()
        {
            LoadConfig();
            if (recyclableType == RecyclableType.OTHERS)
            {
                Debug.LogWarning("The 'OTHER' Recyclable type is meant for non-recyclables / contaminants! Using it on Recyclables will have unintended effects! You should probably use ContaminantNPC instead!");
            }
            if (contaminant_prefab == null)
            {
                Debug.LogWarning("IMPORTANT! contaminant_prefab is a required field! When null, it will cause this recyclable to never spawn it's contaminated version");
            }
        }

        public virtual void Damage(float damage)
        {
            // Recyclables cannot be damaged (by player)
            // They can still be damaged by contaminants.
            // This method is here to satisfy the IDamagable interface.
            // Please do not remove this method.
            // Or remove IDamagable from this class.
        }
    }
}
