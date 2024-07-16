using UnityEngine;
using NPC.Contaminant;

namespace Level.Tutorial
{
    public class TutorialGeneralWasteDisposalTask : TutorialTaskWithInfoBox
    {
        [SerializeField, Range(0f, 1f)] float minHealthPercent = .1f;
        ContaminantNPC contaminant;
        bool completed = false;

        new void Start()
        {
            base.Start();
            if (recyclables == null || recyclables.Length <= 0) return;
            contaminant = recyclables[0].gameObject.GetComponent<ContaminantNPC>();
        }
        
        new void Update()
        {
            base.Update();
            if (completed || contaminant != null) return;
            ResetRecyclables();
        }

        void FixedUpdate()
        {
            if (recyclables == null || recyclables.Length <= 0 || contaminant == null) return;
            contaminant.transform.position = recyclables[0].originalPosition;
        }

        public override bool CheckTaskCompletion()
        {
            if (completed || (contaminant.healthbar.value > minHealthPercent && contaminant != null)) return false;

            if (recyclables != null && recyclables.Length > 0)
            {
                Collider2D hit = Physics2D.OverlapCircle(recyclables[0].originalPosition, 1.5f, LayerMask.GetMask("Recyclable"));
                if (hit != null) 
                {
                    Destroy(hit.gameObject);
                    ResetRecyclables();
                }
            }

            // increment box count
            box.IncrementCount();
            completed = true;
            return true;
        }

        void ResetRecyclables()
        {
            base.ResetRecyclables();
            if (recyclables == null || recyclables.Length <= 0) return;
            contaminant = recyclables[0].gameObject.GetComponent<ContaminantNPC>();
        }
    }
}
