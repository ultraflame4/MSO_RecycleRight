using System.Linq;
using UnityEngine;

namespace Level.Tutorial
{
    public class TutorialPushTask : TutorialTaskWithInfoBox
    {
        [SerializeField] GameObject[] enemies;

        new void Start()
        {
            base.Start();
            if (enemies != null && count == enemies.Length) return;
            count = enemies.Length;
        }

        public override bool CheckTaskCompletion()
        {
            int aliveEnemies = enemies.Where(x => x != null).ToArray().Length;
            box.SetCount(count - aliveEnemies);
            return aliveEnemies <= 0;
        }
    }
}
