using UnityEngine;

namespace Level.Tutorial
{
    public class TutorialTimedTask : TutorialButtonPressTask
    {
        [Header("Timed Task")]
        [SerializeField] float duration = 2.5f;
        float timePassed;

        new void Start()
        {
            base.Start();
            timePassed = 0f;
        }

        public override bool CheckTaskCompletion()
        {
            if (base.CheckTaskCompletion()) return true;
            timePassed += Time.deltaTime;
            return timePassed >= duration;
        }
    }
}
