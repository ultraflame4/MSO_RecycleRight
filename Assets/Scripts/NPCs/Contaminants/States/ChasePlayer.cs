using Level;
using NPC;
using NPC.Contaminant;
using NPC.Recyclable;
using Patterns.FSM;
using Player;
using UnityEngine;

namespace NPC.Contaminants.States
{
    public class ChasePlayer : DetectTarget
    {
        public ChasePlayer(ContaminantNPC npc) : base(npc)
        {
        }


        public override void Enter()
        {
            base.Enter();
            navigation.ClearDestination();
            navigation.SetDestination(PlayerController.Instance.transform);
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            if (!npc.playerInSight)
            {
                npc.SwitchState(npc.state_Idle);
                return;
            }

            if (npc.playerInAttackRange)
            {
                npc.SwitchState(npc.state_AttackPlayer);   
                return;
            }
        }

        public override void Exit()
        {
            base.Exit();
            navigation.ClearDestination();
        }
    }
}