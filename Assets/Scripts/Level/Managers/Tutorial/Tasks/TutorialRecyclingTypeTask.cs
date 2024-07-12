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

        // Update is called once per frame
        new void Update()
        {
            base.Update();
            CheckReset();
        }

        public override bool CheckTaskCompletion()
        {
            // update information box count UI based on number of recyclables that are destroyed (in the bin)
            box.SetCount(recyclables.Select(x => x.gameObject).Where(x => x == null).ToArray().Length);
            // ensure all recyclables are destroyed
            foreach (Recyclable recyclable in recyclables)
            {
                if (recyclable.gameObject == null) continue;
                return false;
            }
            // ensure all bins are clean
            foreach (RecyclingBin bin in bins)
            {
                if (bin.binState == BinState.CLEAN) continue;
                return false;
            }
            return true;
        }

        void CheckReset()
        {
            foreach (RecyclingBin bin in bins)
            {
                if (bin.binState == BinState.CLEAN) continue;
                bin.CompleteClean();
                bin.Score = 0;
                ResetRecyclables();
            }
        }

        void ResetRecyclables()
        {
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
