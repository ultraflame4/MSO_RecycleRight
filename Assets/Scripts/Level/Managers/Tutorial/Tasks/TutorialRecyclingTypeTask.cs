using System.Linq;
using UnityEngine;
using Level.Bins;

namespace Level.Tutorial
{
    public class TutorialRecyclingTypeTask : TutorialTaskWithInfoBox
    {
        [SerializeField] GameObject[] recyclables;
        [SerializeField] GameObject[] recyclablePrefabs;
        [SerializeField] RecyclingBin[] bins;

        Vector3[] originalRecyclablePositions;

        // Start is called before the first frame update
        new void Start()
        {
            base.Start();
            // cache recyclable positions
            originalRecyclablePositions = recyclables.Select(x => x.transform.position).ToArray();
        }

        // Update is called once per frame
        new void Update()
        {
            base.Update();
        }

        public override bool CheckTaskCompletion()
        {
            return false;
        }
    }
}
