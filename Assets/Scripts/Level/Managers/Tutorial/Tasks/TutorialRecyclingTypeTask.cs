using System;
using System.Linq;
using UnityEngine;
using Level.Bins;
using NPC;

namespace Level.Tutorial
{
    public class TutorialRecyclingTypeTask : TutorialTaskWithInfoBox
    {
        [SerializeField] Recyclable[] recyclables;
        [SerializeField] RecyclingBin[] bins;

        [Serializable]
        private struct Recyclable
        {
            public GameObject gameObject;
            public GameObject prefab;
        }

        Vector3[] originalRecyclablePositions;
        Transform recyclableParent;

        // Start is called before the first frame update
        new void Start()
        {
            base.Start();
            // cache recyclable positions and parent
            originalRecyclablePositions = recyclables.Select(x => x.gameObject.transform.position).ToArray();
            if (recyclables != null) recyclableParent = recyclables[0].gameObject.transform.parent;
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

            for (int i = 0; i < recyclables.Length; i++)
            {
                Destroy(recyclables[i].gameObject);
                recyclables[i].gameObject = Instantiate(
                        recyclables[i].prefab, 
                        originalRecyclablePositions[i], 
                        Quaternion.identity, 
                        recyclableParent
                    );
                recyclables[i].gameObject.GetComponent<Navigation>().enabled = false;
            }
        }
    }
}
