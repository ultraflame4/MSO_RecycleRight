using NPC.Contaminant;
using UnityEngine;

public class ContaminantAnimationEvents : MonoBehaviour {
    public ContaminantNPC contaminantNPC;
    public void TriggerHit()
    {
        contaminantNPC.state_AttackPlayer.TriggerHit();
        contaminantNPC.state_AttackRecyclable.TriggerHit();
    }
    public void TriggerAttackEnd()
    {
        contaminantNPC.state_AttackPlayer.EndAttack();
        contaminantNPC.state_AttackRecyclable.EndAttack();
    }
    public void TriggerEnd()
    {
        TriggerAttackEnd();
    }
    
}