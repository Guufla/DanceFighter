using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundEntryState : AttackBase
{
	public AttackType activeAttackType;
	private ComboSequence combo;
	
	public GroundEntryState(AttackType attackType)
	{
		activeAttackType = attackType;
	}
	
	private ComboSequence GetComboBasedOnType(AttackType currentAttackType)
	{
		switch (currentAttackType)
		{
			case AttackType.attackE:
				return GetLightCombo();
			case AttackType.attackR:
				return GetMediumCombo();
			default:
				return null;
		}
	}

    public override void OnEnter(StateMachine stateMachine)
	{
		stateMachine.attackIndex = 0;
		int index = stateMachine.attackIndex;
		combo = GetComboBasedOnType(activeAttackType);
		attackAnimator.SetTrigger(combo.steps[index].animationName);
		EnableBuffering();
	}
	
	public override void OnUpdate()
	{
		int index = stateMachine.attackIndex;
		if (fixedTime >= combo.steps[index].duration)
		{
			if (combo.steps[index].canCombo && stateMachine.attackQueue.Count > 0)
			{
				stateMachine.SetNextState(new GroundComboState(stateMachine.attackQueue.Dequeue()));
			} else {
				stateMachine.SetNextState(new IdleState());
			}
		}
	}
}