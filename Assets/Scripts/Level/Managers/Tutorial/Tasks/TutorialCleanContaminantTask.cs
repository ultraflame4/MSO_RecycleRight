using UnityEngine;
using NPC.Contaminant;

namespace Level.Tutorial
{
    public class TutorialCleanContaminantTask : TutorialTaskWithInfoBox
    {
        [SerializeField] Transform zone;
        [SerializeField] ContaminantNPC contaminant;
        [SerializeField] float minGrimeAmount = .15f;
        Vector3 orignalContaminantPosition;
        bool completed = false;

        // Start is called before the first frame update
        new void Start()
        {
            base.Start();
            orignalContaminantPosition = contaminant.transform.position;
        }

        void FixedUpdate()
        {
            if (contaminant == null) return;
            contaminant.transform.position = orignalContaminantPosition;
        }

        public override bool CheckTaskCompletion()
        {
            if (completed || contaminant.grimeController.GrimeAmount > minGrimeAmount) return false;

            // clean up game objects after completing task
            if (contaminant != null) Destroy(contaminant.gameObject);

            if (zone != null)
            {
                Collider2D hit = Physics2D.OverlapCircle(zone.position, 5f, LayerMask.GetMask("Recyclable"));
                if (hit != null) Destroy(hit.gameObject);
            }

            // increment box count
            box.IncrementCount();
            completed = true;
            return true;
        }

        void OnDrawGizmosSelected() 
        {
            if (zone == null) return;
            Gizmos.DrawWireSphere(zone.position, 5f);
        }
    }
}
