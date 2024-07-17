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
            int recyclableCount = recyclables
                .Select(x => x.gameObject)
                .Where(x => x == null)
                .ToArray().Length;
            
            // update information box count UI based on number of recyclables that are destroyed (in the bin)
            box.SetCount(recyclableCount);

            // ensure all bins are clean
            foreach (RecyclingBin bin in bins)
            {
                if (bin.binState == BinState.CLEAN) 
                    continue;
                if (recyclableCount >= (recyclables.Length - 1)) 
                    ResetRecyclables();
                return false;
            }
            return recyclableCount == recyclables.Length;
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
