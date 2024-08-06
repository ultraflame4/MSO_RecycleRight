using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bosses.Pilotras.FSM;
using Patterns.FSM;

namespace Bosses.Pilotras
{
    public class PilotrasController : StateMachine<PilotrasController>
    {
        #region Data
        [field: SerializeField] public PilotrasData data { get; private set; }
        [field: SerializeField] public PilotrasBehaviourData behaviourData { get; private set; }
        #endregion

        #region States
        public DefaultState DefaultState { get; private set; }
        public PlacingState PlacingState { get; private set; }
        #endregion

        void Start()
        {
            // initialize states
            DefaultState = new DefaultState(this, this);
            PlacingState = new PlacingState(this, this);
            // initialize FSM
            Initialize(DefaultState);
        }

        new void Update()
        {
            if (Time.timeScale <= 0f) return;
            base.Update();
        }

        new void FixedUpdate()
        {
            if (Time.timeScale <= 0f) return;
            base.FixedUpdate();
        }
    }
}

