using UnityEngine;
using Bosses.Pilotras;

namespace Level.Tutorial
{
    public class PilotrasLevelManager : LevelManager
    {
        [Header("Pilotras Level")]
        [Tooltip("Reference to Pilotras controller.")]
        [SerializeField] PilotrasController boss;
        [Tooltip("Minimum time before score starts decreasing in minutes.")]
        [SerializeField] float minCompletionTime = 5f;
        float timeElasped;

        public override void Start()
        {
            base.Start();
            timeElasped = 0f;

            if (boss == null)
            {
                Debug.LogWarning("Boss is null, level cannot be ended! (PilotrasLevelManager.cs)");
                return;
            }

            boss.DeathState.EndLevel += EndLevel;
        }

        void Update()
        {
            timeElasped += Time.deltaTime;
        }

        public override float GetCurrentScore()
        {
            float score = base.GetCurrentScore();

            if (timeElasped >= minCompletionTime * 60f)
            {
                // reduce score by 1 for every score after minute
                score -= Mathf.RoundToInt((timeElasped - (minCompletionTime * 60f)) / 60f);
                if (score < 0f) score = 0f;
            }

            return score;
        }

        public void EndLevel()
        {
            base.EndLevel();
            boss.DeathState.EndLevel -= EndLevel;
        }
    }
}
