using UnityEngine;
using Level.Bins;

namespace Level.Tutorial
{
    public class TutorialBinCleaningTask : TutorialTaskWithInfoBox
    {
        [SerializeField] RecyclingBin bin;

        public override bool CheckTaskCompletion()
        {
            if (bin.binState == BinState.CLEAN) box.SetCount(1);
            return bin.binState == BinState.CLEAN;
        }
    }
}
