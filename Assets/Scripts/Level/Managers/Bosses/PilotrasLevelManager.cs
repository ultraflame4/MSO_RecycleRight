using UnityEngine;
using Bosses.Pilotras;

namespace Level.Tutorial
{
    public class PilotrasLevelManager : MonoBehaviour
    {
        [Header("Pilotras Level")]
        [Tooltip("Reference to Pilotras controller.")]
        [SerializeField] PilotrasController boss;
        [Tooltip("Minimum time before score starts decreasing in minutes.")]
        [SerializeField] float minCompletionTime = 5f;
        
        void Start()
        {
            if (boss == null)
            {
                Debug.LogWarning("Boss is null, level cannot be ended! (PilotrasLevelManager.cs)");
                return;
            }

            boss.DeathState.EndLevel += EndLevel;
        }

        void EndLevel()
        {
            if (LevelManager.Instance == null)
            {
                Debug.LogWarning("Level manager instance not found, unable to end level. (PilotrasLevelManager.cs)");
                return;
            }

            LevelManager.Instance.EndLevel();
            boss.DeathState.EndLevel -= EndLevel;
        }
    }
}
