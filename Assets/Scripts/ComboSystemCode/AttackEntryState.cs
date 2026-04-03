using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackEntryState : State
{
    public override void OnEnter(StateMachine stateMachine)
    {
        base.OnEnter(stateMachine);

        //State nextState = (State)new GroundEntryState();
        //stateMachine.SetNextState(nextState);
    }
}