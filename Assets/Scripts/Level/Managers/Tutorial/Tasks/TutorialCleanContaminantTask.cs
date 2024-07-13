using UnityEngine;
using NPC.Contaminant;

namespace Level.Tutorial
{
    public class TutorialCleanContaminantTask : TutorialTaskWithInfoBox
    {
        [SerializeField] ContaminantNPC contaminant;
        [SerializeField] float minGrimeAmount = .15f;

        // Start is called before the first frame update
        new void Start()
        {
            base.Start();
        }

        // Update is called once per frame
        new void Update()
        {
            base.Update();
        }

        public override bool CheckTaskCompletion()
        {
            if (contaminant.grimeController.GrimeAmount > minGrimeAmount) return false;
            Destroy(contaminant.gameObject);
            return true;
        }
    }
}
