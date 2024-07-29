using System.Linq;
using UnityEngine;
using Level.Bins;

namespace Level.Tutorial
{
    public class TutorialBinContaminationTask : TutorialTask
    {
        [SerializeField] RecyclingBin[] bins;
        bool completed = false;

        new void Update()
        {
            base.Update();
            // check if need to reset contaminant
            if (completed || recyclables == null || recyclables.Length <= 0 || !(recyclables[0].gameObject == null && 
                bins.Where(x => x.binState == BinState.CLEAN).ToArray().Length == bins.Length)) 
                    return;
            // reset recyclables
            ResetRecyclables();
            // ensure cleaned contaminant does not spawn a recyclable
            LevelZone currZone = LevelManager.Instance.zones[LevelManager.Instance.current_zone_index];
            Collider2D hit = Physics2D.OverlapBox(currZone.transform.position, 
                currZone.size, LayerMask.GetMask("Recyclable"));
            if (hit == null) return; 
            Destroy(hit.gameObject);
        }

        public override bool CheckTaskCompletion()
        {
            foreach (RecyclingBin bin in bins)
            {
                if (bin.binState != BinState.CONTAMINATED) continue;
                completed = true;
                return true;
            }
            return false;
        }
    }
}
