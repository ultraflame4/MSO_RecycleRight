using System.Linq;
using Level;
using Level.Bins;
using NPC;
using NPC.Contaminant;
using NPC.Recyclable;
using Patterns.FSM;
using Player;
using UnityEngine;

namespace NPC.Contaminants.States
{
    public class ChaseBin : DetectTarget
    {

        public RecyclingBin targetBin;
        public ChaseBin(ContaminantNPC npc) : base(npc)
        {
        }


        public override void Enter()
        {
            base.Enter();
            navigation.ClearDestination();
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            // todo, use layer mask to filter colliders
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, npc.sightRange);
            var nearest = colliders.Select(x=>x.GetComponent<RecyclingBin>()).Where(x=>x!=null).OrderBy(x=>(x.transform.position-npc.transform.position).sqrMagnitude).FirstOrDefault();
            if (nearest){
                navigation.SetDestination(nearest.transform);
                return; 
            }
            npc.SwitchState(npc.state_Idle);
        }

        public override void Exit()
        {
            base.Exit();
            navigation.ClearDestination();
        }
    }
}