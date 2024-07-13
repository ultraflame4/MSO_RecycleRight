using Entity.Data;
using Player;
using Behaviour = Player.Behaviours.Behaviour;

namespace Level.Tutorial
{
    public class TutorialSkillTask : TutorialTaskWithInfoBox
    {
        CharacterManager characterManager;
        Behaviour behaviour;
        bool conditionTriggered, tutorialActivated = false;
        int currentCount;

        // Start is called before the first frame update
        new void Start()
        {
            base.Start();
            currentCount = 0;
            characterManager = PlayerController.Instance.CharacterManager;
        }

        new void Update()
        {
            // check for activating tutorial
            if (!tutorialActivated && IsActive)
            {
                tutorialActivated = true;
                characterManager.CharacterChanged += OnCharacterChange;
                if (behaviour == null) behaviour = PlayerController.Instance.CharacterBehaviour;
                behaviour.SkillTriggered += SkillTriggered;
                PlayerController.Instance.Data.skillCooldownMultiplier = 0f;
            }
            base.Update();
        }
        
        public override bool CheckTaskCompletion()
        {
            if (!conditionTriggered) return false;
            currentCount++;
            conditionTriggered = false;
            box.IncrementCount();
            if (count != 0 && currentCount < count) return false;
            PlayerController.Instance.Data.skillCooldownMultiplier = 1f;
            characterManager.CharacterChanged -= OnCharacterChange;
            behaviour.SkillTriggered -= SkillTriggered;
            return true;
        }

        void OnCharacterChange(PlayerCharacter prev, PlayerCharacter curr)
        {
            curr.skillCooldownMultiplier = 0f;
            behaviour = curr.GetComponent<Behaviour>();
            if (behaviour != null) behaviour.SkillTriggered += SkillTriggered;
            if (prev == null) return;
            prev.skillCooldownMultiplier = 1f;
            behaviour = prev.GetComponent<Behaviour>();
            if (behaviour != null) behaviour.SkillTriggered -= SkillTriggered;
        }

        void SkillTriggered(PlayerCharacter data)
        {
            conditionTriggered = true;
        }
    }
}
