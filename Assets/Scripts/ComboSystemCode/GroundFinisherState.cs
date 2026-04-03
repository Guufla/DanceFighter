using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundFinisherState : AttackBase
{
	private AttackType activeAttackType;
	private ComboSequence combo;

	public GroundFinisherState(AttackType attackType)
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
		base.OnEnter(stateMachine);
		stateMachine.attackIndex++;
		int index = stateMachine.attackIndex;
		Debug.Log("Finisher OnEnter attackIndex: " + index);

		combo = GetComboBasedOnType(activeAttackType);
		if(combo == null || combo.steps == null || index < 0 || index >= combo.steps.Length)
		{
			Debug.LogError("Invalid combo configuration for attack type: " + activeAttackType);
			stateMachine.SetNextState(new IdleState());
			return;
		}
		attackAnimator.SetTrigger(combo.steps[index].animationName);
		DisableBuffering();
	}

	public override void OnUpdate()
	{
		base.OnUpdate();

		int index = stateMachine.attackIndex;
		if(combo != null && combo.steps != null && index >= 0 && index < combo.steps.Length) {
			if (fixedTime >= combo.steps[index].duration)
			{
				stateMachine.SetNextState(new IdleState());
				stateMachine.attackIndex = 0;
			}
		}	
	}
}