using System.Linq;
using UnityEngine;
using Level.Bins;
using NPC;

namespace Level.Tutorial
{
    public class TutorialPushTask : TutorialTaskWithInfoBox
    {
        [SerializeField] RecyclingBin bin;
        [SerializeField] GameObject enemyPrefab;
        [SerializeField] GameObject[] enemies;
        Vector3[] originalEnemyPositions;
        Transform originalParent;

        new void Start()
        {
            base.Start();

            originalParent = enemies[0].transform.parent;
            originalEnemyPositions = enemies
                .Select(x => x.transform.position)
                .ToArray();

            if (enemies != null && count == enemies.Length) return;
            count = enemies.Length;
        }

        public override bool CheckTaskCompletion()
        {
            int disposedEnemies = enemies.Where(x => x == null).ToArray().Length;
            box.SetCount(disposedEnemies);
            if (bin != null && disposedEnemies > bin.Score)
            {
                Reset();
                return false;
            }
            return disposedEnemies >= count;
        }

        void Reset()
        {
            for (int i = 0; i < enemies.Length; i++)
            {
                if (enemies[i] != null) Destroy(enemies[i]);
                enemies[i] = Instantiate(
                        enemyPrefab, 
                        originalEnemyPositions[i], 
                        Quaternion.identity, 
                        originalParent
                    );
                enemies[i].GetComponent<Navigation>().enabled = false;
            }
            bin.Score = 0;
        }
    }
}
