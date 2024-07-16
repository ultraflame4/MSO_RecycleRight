using UnityEngine;
using NPC;
using NPC.Contaminant;

namespace Level.Tutorial
{
    public class TutorialCleanContaminantTask : TutorialTaskWithInfoBox
    {
        [SerializeField] float minGrimeAmount = .15f;
        ContaminantNPC contaminant;
        Collider2D hit;
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
            if (!completed && contaminant == null)
            {
                ResetRecyclables();
                if (recyclables == null || recyclables.Length <= 0) return;
                contaminant = recyclables[0].gameObject.GetComponent<ContaminantNPC>();
            }
        }

        void FixedUpdate()
        {
            if (recyclables == null || recyclables.Length <= 0) return;
            if (hit != null) hit.transform.position = recyclables[0].originalPosition;
            if (contaminant == null) return;
            contaminant.transform.position = recyclables[0].originalPosition;
        }

        public override bool CheckTaskCompletion()
        {
            if (completed || contaminant.grimeController.GrimeAmount > minGrimeAmount) return false;

            // clean up game objects after completing task
            if (contaminant != null) Destroy(contaminant.gameObject);

            if (recyclables != null && recyclables.Length > 0)
            {
                Collider2D hit = Physics2D.OverlapCircle(recyclables[0].originalPosition, 1.5f, LayerMask.GetMask("Recyclable"));
                if (hit != null) 
                {
                    hit.GetComponent<Navigation>().enabled = false;
                    TaskCompleted += DestroyRecyclable;
                }
            }

            // increment box count
            box.IncrementCount();
            completed = true;
            return true;
        }

        void DestroyRecyclable()
        {
            if (hit == null) return;
            Destroy(hit.gameObject);
            TaskCompleted -= DestroyRecyclable;
        }
    }
}
