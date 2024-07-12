using Player;
using Entity.Data;

namespace Level.Tutorial
{
    public class TutorialSwitchTask : TutorialTaskWithInfoBox
    {
        CharacterManager characterManager;
        bool switchTriggered = false;
        int currentCount;

        // Start is called before the first frame update
        new void Start()
        {
            base.Start();
            currentCount = 0;
            characterManager = PlayerController.Instance.CharacterManager;
            characterManager.CharacterChanged += OnCharacterChange;
        }

        public override bool CheckTaskCompletion()
        {
            if (!switchTriggered) return false;
            currentCount++;
            switchTriggered = false;
            box.IncrementCount();
            if (count != 0 && currentCount < count) return false;
            characterManager.CharacterChanged -= OnCharacterChange;
            return true;
        }

        void OnCharacterChange(PlayerCharacter prev, PlayerCharacter curr)
        {
            if (prev == null || prev == curr) return;
            switchTriggered = true;
        }
    }
}
