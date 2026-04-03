using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : State
{
    public override void OnEnter(StateMachine stateMachine)
	{
		base.OnEnter(stateMachine);
		stateMachine.animator.Play("Idle");
		stateMachine.attackIndex = 0;
		stateMachine.attackQueue.Clear();
		stateMachine.attackBuffer = 0;
		stateMachine.isBuffering = false;
	}
}