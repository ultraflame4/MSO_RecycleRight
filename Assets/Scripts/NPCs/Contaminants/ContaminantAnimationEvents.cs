using NPC.Contaminant;
using UnityEngine;

public class ContaminantAnimationEvents : MonoBehaviour {
    public ContaminantNPC contaminantNPC;
    public void TriggerHit()
    {
        contaminantNPC.state_AttackPlayer.TriggerHit();
    }
    public void TriggerAttackEnd()
    {
        contaminantNPC.state_AttackPlayer.EndAttack();
    }
        public void TriggerEnd()
    {
        TriggerAttackEnd();
    }
    
}