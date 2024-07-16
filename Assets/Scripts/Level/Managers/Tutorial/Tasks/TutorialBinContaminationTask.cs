using System.Linq;
using UnityEngine;
using Level.Bins;
using NPC;

namespace Level.Tutorial
{
    public class TutorialBinContaminationTask : TutorialTask
    {
        [SerializeField] GameObject contaminant;
        [SerializeField] GameObject contaminantPrefab;
        [SerializeField] RecyclingBin[] bins;
        Transform originalParent;
        Vector3 originalContaminantPosition;

        new void Start()
        {
            base.Start();
            originalParent = contaminant.transform.parent;
            originalContaminantPosition = contaminant.transform.position;
        }

        new void Update()
        {
            base.Update();

            if (contaminant == null && bins.Where(x => x.binState == BinState.CLEAN).ToArray().Length == bins.Length)
            {
                contaminant = Instantiate(
                        contaminantPrefab, 
                        originalContaminantPosition, 
                        Quaternion.identity, 
                        originalParent
                    );
                contaminant.GetComponent<Navigation>().enabled = false;

                // ensure cleaned contaminant does not spawn a recyclable
                Collider2D hit = Physics2D.OverlapCircle(contaminant.transform.position, 5f, LayerMask.GetMask("Recyclable"));
                if (hit != null) Destroy(hit.gameObject);
            }
        }

        public override bool CheckTaskCompletion()
        {
            
            foreach (RecyclingBin bin in bins)
            {
                if (bin.binState != BinState.CONTAMINATED) continue;
                return true;
            }
            return false;
        }
    }
}
