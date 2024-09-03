using UnityEngine;
using Patterns.FSM;
using Level.Bins;
using UnityEngine.Serialization;
using TMPro;

namespace NPC
{
    public class FSMRecyclableNPC : StateMachine<FSMRecyclableNPC>, IBinTrashItem, ILevelEntity
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
        public TextMeshProUGUI nameText;
        public AudioClip enterBin;
        public AudioClip enterGeneralWaste;

        protected virtual void SpawnRecyclable() { }

        public void OnEnterBin(RecyclingBin bin)
        {
            if (bin.binState != BinState.CLEAN) return;

            // play sound effect
            SoundManager.Instance?.PlayOneShot(enterBin);

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
            // gameObject.SetActive(false);
            Destroy(gameObject);
        }

        /// <summary>
        /// Sets the name tag of the NPC.
        /// </summary>
        /// <param name="always">When false, If trimmed (removing leading and trailing spaces) name is empty (length < 1), the name tag will not be set.</param>
        /// <param name="name"></param>
        protected void SetNameTag(string name,bool always = true)
        {
            if (nameText)
            {
                var trimmed = name.Trim(' ');
                if (trimmed.Length > 0 || always)
                {
                    nameText.text = trimmed;
                }
            }
        }

        public void OnZoneEnd()
        {
            // This may already be destroyed, so no need to do anything else.
            if (this == null) return;
            gameObject.SetActive(false);
        }

        public void OnZoneStart()
        {
            gameObject.SetActive(true);
        }
    }
}

