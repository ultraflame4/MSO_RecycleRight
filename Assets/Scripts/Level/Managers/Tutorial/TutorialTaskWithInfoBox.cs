using UnityEngine;
using UI;

namespace Level.Tutorial
{
    public abstract class TutorialTaskWithInfoBox : TutorialTask
    {
        [Header("Information Box"), TextArea(3, 10)]
        [SerializeField] protected string text;
        [SerializeField] protected int count = 0;
        [SerializeField] protected InformationBox box;
        [SerializeField] protected bool showBox = true;

        public override void SetTutorialActive(bool active)
        {
            base.SetTutorialActive(active);
            if (box == null || !active) return;
            box.SetInformation(text, count);
            box.ResetCount();
            box.UpdateCountUI();
            box.gameObject.SetActive(showBox);
        }

        public abstract override bool CheckTaskCompletion();
    }
}
