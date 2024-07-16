using UnityEngine;
using Level.Bins;

namespace Level.Tutorial
{
    public class TutorialBinCleaningTask : TutorialTaskWithInfoBox
    {
        [SerializeField] RecyclingBin[] bins;

        public override bool CheckTaskCompletion()
        {
            foreach (RecyclingBin bin in bins)
            {
                if (bin.binState != BinState.CLEAN) continue;
                box.SetCount(1);
                return true;
            }
            return false;
        }
    }
}
