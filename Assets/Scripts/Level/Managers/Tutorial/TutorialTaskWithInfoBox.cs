using UnityEngine;
using UI;

namespace Level.Tutorial
{
    public abstract class TutorialTaskWithInfoBox : TutorialTask
    {
        [Header("Information Box"), TextArea(3, 10)]
        [SerializeField] string text;
        [SerializeField] int count = 0;
        [SerializeField] InformationBox box;

        // Start is called before the first frame update
        new void Start()
        {
            if (box == null) return;
            box.SetInformation(text, count);
            // start base class
            base.Start();
        }

        public abstract override bool CheckTaskCompletion();
    }
}
