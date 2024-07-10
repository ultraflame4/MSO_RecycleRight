using UnityEngine;
using Patterns.FSM;
using Level.Bins;
using UnityEngine.Serialization;

namespace NPC
{
    public class FSMRecyclableNPC : StateMachine<FSMRecyclableNPC>
    {
        [field: SerializeField]
        public Navigation navigation {get; private set;}
        /// <summary>
        /// The recyclable type of this NPC (if contamiant NPC, set to OTHERS)
        /// </summary>
        public virtual RecyclableType recyclableType { get; }
        /// <summary>
        /// Make this return true if this NPC is a food item. (Causes infestation)
        /// </summary>
        public virtual bool is_food { get; }
        [SerializeField, FormerlySerializedAs("animator")]
        private Animator _animator;
        public Animator animator {
            get {
                if (_animator) return _animator;
                return null;
            }
        }
        public SpriteRenderer spriteRenderer;

        public virtual void OnEnteredBin(RecyclingBin bin){
            if (is_food){
                bin.StartInfestation();
            }
            else if ( recyclableType != bin.recyclableType){
                bin.SetContaminated();
            }
            else{
                bin.Score += 1;
            }
            // Destroy this NPC. In future if we want death animation we canb remove this
            Destroy(gameObject);
        }

    }
}

