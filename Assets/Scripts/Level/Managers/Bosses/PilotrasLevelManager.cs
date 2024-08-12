using UnityEngine;
using Bosses.Pilotras;

namespace Level.Tutorial
{
    public class PilotrasLevelManager : MonoBehaviour
    {
        [SerializeField] PilotrasController boss;

        void Start()
        {
            if (boss == null)
            {
                Debug.LogWarning("Boss is null, level cannot be ended! (PilotrasLevelManager.cs)");
                return;
            }

            boss.PhaseChangeState.EndLevel += EndLevel;
        }

        void EndLevel()
        {
            if (LevelManager.Instance == null)
            {
                Debug.LogWarning("Level manager instance not found, unable to end level. (PilotrasLevelManager.cs)");
                return;
            }

            LevelManager.Instance.EndLevel();
            boss.PhaseChangeState.EndLevel -= EndLevel;
        }
    }
}
