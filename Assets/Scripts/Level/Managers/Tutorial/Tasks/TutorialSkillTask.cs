using UnityEngine;
using Entity.Data;
using Player;
using Behaviour = Player.Behaviours.Behaviour;

namespace Level.Tutorial
{
    public class TutorialSkillTask : TutorialTaskWithInfoBox
    {
        CharacterManager characterManager;
        Behaviour behaviour;
        bool conditionTriggered = false;
        int currentCount;

        // Start is called before the first frame update
        new void Start()
        {
            base.Start();
            currentCount = 0;
            characterManager = PlayerController.Instance.CharacterManager;
            behaviour = PlayerController.Instance.CharacterBehaviour;
            characterManager.CharacterChanged += OnCharacterChange;
            if (behaviour == null) return;
            behaviour.SkillTriggered += SkillTriggered;
        }
        
        public override bool CheckTaskCompletion()
        {
            Debug.Log("test");
            PlayerController.Instance.Data.skillCooldownMultiplier = 0f;
            if (!conditionTriggered) return false;
            currentCount++;
            conditionTriggered = false;
            box.IncrementCount();
            PlayerController.Instance.Data.skillCooldownMultiplier = 1f;
            if (count != 0 && currentCount < count) return false;
            characterManager.CharacterChanged -= OnCharacterChange;
            return true;
        }

        void OnCharacterChange(PlayerCharacter prev, PlayerCharacter curr)
        {
            if (!IsActive) return;
            behaviour = curr.GetComponent<Behaviour>();
            if (behaviour != null) behaviour.SkillTriggered += SkillTriggered;
            if (prev == null) return;
            prev.skillCooldownMultiplier = 1f;
            behaviour = prev.GetComponent<Behaviour>();
            if (behaviour != null) behaviour.SkillTriggered -= SkillTriggered;
        }

        void SkillTriggered(PlayerCharacter data)
        {
            if (!IsActive) return;
            conditionTriggered = true;
        }
    }
}
