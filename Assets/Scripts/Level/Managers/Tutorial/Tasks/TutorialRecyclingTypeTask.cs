using System.Linq;
using UnityEngine;
using Level.Bins;

namespace Level.Tutorial
{
    public class TutorialRecyclingTypeTask : TutorialTaskWithInfoBox
    {
        [SerializeField] RecyclingBin[] bins;

        public override bool CheckTaskCompletion()
        {
            int recyclableCount = recyclables
                .Select(x => x.gameObject)
                .Where(x => x == null)
                .ToArray().Length;
            
            int totalCleanBins = bins
                .Where(x => x.binState == BinState.CLEAN)
                .ToArray().Length;
            
            int totalScore = (int) Enumerable.Sum(bins.Select(x => x.Score).ToArray());
            
            // update information box count UI based on number of recyclables that are destroyed (in the bin)
            box.SetCount(recyclableCount);

            // check for completion condition
            if (totalCleanBins == bins.Length && totalScore == recyclables.Length)
                return true;
            // check if need to reset task
            if (totalCleanBins < bins.Length || (totalScore < recyclables.Length && totalScore < recyclableCount)) 
                ResetRecyclables();

            return false;
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
