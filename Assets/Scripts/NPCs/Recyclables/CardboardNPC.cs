using System.Collections;
using Interfaces;
using NPC.States;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace NPC.Recyclable
{
    public class CardboardNPC : RecyclableNPC, ICleanable
    {
        public bool AllowCleanable => false;

        public void Clean(float clean_amount)
        {
        }

        protected override void Start()
        {
            base.Start();
            state_Idle = new NothingState(this, this);
            Initialize(state_Idle);
        }
    }
}