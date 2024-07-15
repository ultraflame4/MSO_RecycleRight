using UnityEngine;
using Patterns.FSM;
using Level.Bins;
using UnityEngine.Serialization;

namespace NPC
{
    public class FSMRecyclableNPC : StateMachine<FSMRecyclableNPC>
    {
        [field: SerializeField]
        public Navigation navigation { get; private set; }
        /// <summary>
        /// The recyclable type of this NPC (if contamiant NPC, set to OTHERS)
        /// </summary>
        public virtual RecyclableType recyclableType { get; }
        /// <summary>
        /// Make this return true if this NPC causes infestation in the bin.
        /// Note that if this is true, recyclableType should be set to OTHERS as the NPC should be considered a "contaminant" and cannot be recycled.
        /// </summary>
        public virtual bool cause_infestation { get; } = false;
        
        [SerializeField, FormerlySerializedAs("animator")]
        private Animator _animator;
        public Animator animator
        {
            get
            {
                if (_animator) return _animator;
                return null;
            }
        }
        public SpriteRenderer spriteRenderer;

        public virtual void OnEnteredBin(RecyclingBin bin)
        {
            if (cause_infestation)
            {
                bin.StartInfestation();
            }
            else if (recyclableType != bin.recyclableType)
            {
                bin.SetContaminated();
            }
            else
            {
                bin.Score += 1;
            }
            // Destroy this NPC. In future if we want death animation we canb remove this
            Destroy(gameObject);
        }

    }
}

