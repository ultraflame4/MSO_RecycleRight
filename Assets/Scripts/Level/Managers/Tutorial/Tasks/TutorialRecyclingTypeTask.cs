using System.Linq;
using UnityEngine;
using Level.Bins;

namespace Level.Tutorial
{
    public class TutorialRecyclingTypeTask : TutorialTaskWithInfoBox
    {
        [SerializeField] RecyclingBin[] bins;

        // Start is called before the first frame update
        new void Start()
        {
            base.Start();
            // reset recyclables
            ResetRecyclables();
        }

        public override bool CheckTaskCompletion()
        {
            int recyclableCount, scoredBins;

            recyclableCount = recyclables
                .Select(x => x.gameObject)
                .Where(x => x != null)
                .ToArray().Length;
            
            scoredBins = bins
                .Where(x => x.binState == BinState.CLEAN && x.Score > 0)
                .ToArray().Length;

            // update information box count UI based on number of recyclables that are destroyed (in the bin)
            box.SetCount(recyclables.Length - recyclableCount);

            // ensure all bins are clean, and have 1 score
            foreach (RecyclingBin bin in bins)
            {
                if (recyclableCount == 0) 
                {
                    if (scoredBins == recyclables.Length) continue;
                    ResetRecyclables();
                }
                return false;
            }
            return true;
        }

        void ResetRecyclables()
        {
            foreach (RecyclingBin bin in bins)
            {
                bin.CompleteClean();
                bin.Score = 0;
            }
            base.ResetRecyclables();
        }
    }
}
