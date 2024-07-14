using UnityEngine;
using NPC;
using NPC.Contaminant;

namespace Level.Tutorial
{
    public class TutorialCleanContaminantTask : TutorialTaskWithInfoBox
    {
        [SerializeField] Transform zone;
        [SerializeField] ContaminantNPC contaminant;
        [SerializeField] GameObject contaminantPrefab;
        [SerializeField] float minGrimeAmount = .15f;
        Vector3 orignalContaminantPosition;
        Transform originalParent;
        Collider2D hit;
        bool completed = false;

        // Start is called before the first frame update
        new void Start()
        {
            base.Start();
            orignalContaminantPosition = contaminant.transform.position;
            originalParent = contaminant.transform.parent;
        }

        new void Update()
        {
            base.Update();
            if (!completed && contaminant == null && contaminantPrefab != null)
            {
                GameObject obj = Instantiate(
                        contaminantPrefab, 
                        orignalContaminantPosition, 
                        Quaternion.identity, 
                        originalParent
                    );
                
                obj.GetComponent<Navigation>().enabled = false;
                contaminant = obj.GetComponent<ContaminantNPC>();
            }
        }

        void FixedUpdate()
        {
            if (hit != null) hit.transform.position = orignalContaminantPosition;
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
                hit = Physics2D.OverlapCircle(zone.position, 5f, LayerMask.GetMask("Recyclable"));
                if (hit != null) TaskCompleted += DestroyRecyclable;
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

        void OnDrawGizmosSelected() 
        {
            if (zone == null) return;
            Gizmos.DrawWireSphere(zone.position, 5f);
        }
    }
}
