using Player;
using Behaviour = Player.Behaviours.Behaviour;

namespace Level.Tutorial
{
    public class TutorialSkillTask : TutorialTaskWithInfoBox
    {
        Behaviour behaviour => PlayerController.Instance.CharacterBehaviour;
        bool conditionTriggered = false;
        int currentCount;

        // Start is called before the first frame update
        new void Start()
        {
            base.Start();
            currentCount = 0;
        }
        
        public override bool CheckTaskCompletion()
        {
            if (IsActive && behaviour.CooldownElasped < PlayerController.Instance.Data.skillCooldown) 
            {
                conditionTriggered = true;
                behaviour.CooldownElasped = PlayerController.Instance.Data.skillCooldown;
            }
            if (!conditionTriggered) return false;
            currentCount++;
            conditionTriggered = false;
            box.IncrementCount();
            if (count != 0 && currentCount < count) return false;
            return true;
        }
    }
}
