using UnityEngine;
using NPC.Contaminant;

namespace Level.Tutorial
{
    public class TutorialCleanContaminantTask : TutorialTaskWithInfoBox
    {
        [SerializeField] ContaminantNPC contaminant;
        [SerializeField] float minGrimeAmount = .15f;
        Vector3 orignalContaminantPosition, zonePosition;

        // Start is called before the first frame update
        new void Start()
        {
            base.Start();
            orignalContaminantPosition = contaminant.transform.position;
            if (transform.parent == null) return;
            zonePosition = transform.parent.position;
        }

        void FixedUpdate()
        {
            if (contaminant == null) return;
            contaminant.transform.position = orignalContaminantPosition;
        }

        public override bool CheckTaskCompletion()
        {
            if (contaminant.grimeController.GrimeAmount > minGrimeAmount) return false;
            // clean up game objects after completing task
            if (contaminant != null) Destroy(contaminant.gameObject);
            if (zonePosition != null)
            {
                Collider2D hit = Physics2D.OverlapCircle(zonePosition, 5f, LayerMask.GetMask("Recyclables"));
                if (hit != null) Destroy(hit.gameObject);
            }
            // increment box count
            box.IncrementCount();
            return true;
        }

        void OnDrawGizmosSelected() 
        {
            Gizmos.DrawWireSphere(transform.parent.position, 5f);
        }
    }
}
