using System.Linq;
using UnityEngine;
using Level.Bins;

namespace Level.Tutorial
{
    public class TutorialPushTask : TutorialTaskWithInfoBox
    {
        [SerializeField] RecyclingBin bin;

        new void Start()
        {
            base.Start();
            if (recyclables != null && count == recyclables.Length) return;
            count = recyclables.Length;
        }

        public override bool CheckTaskCompletion()
        {
            int disposedEnemies = recyclables
                .Select(x => x.gameObject)
                .Where(x => x == null)
                .ToArray().Length;
            box.SetCount(disposedEnemies);

            if (bin != null && disposedEnemies > bin.Score)
            {
                ResetRecyclables();
                return false;
            }

            return disposedEnemies >= count;
        }

        void ResetRecyclables()
        {
            base.ResetRecyclables();
            bin.Score = 0;
        }
    }
}
