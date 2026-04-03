using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundComboState : AttackBase
{
	private AttackType activeAttackType;
	private ComboSequence combo;

	public GroundComboState(AttackType attackType)
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
        stateMachine.attackIndex = stateMachine.attackIndex + 1;
        int index = stateMachine.attackIndex;
        combo = GetComboBasedOnType(activeAttackType);
        attackAnimator.SetTrigger(combo.steps[index].animationName);
        EnableBuffering();
    }

	public override void OnUpdate()
	{
		int index = stateMachine.attackIndex;
		
		if (fixedTime >= combo.steps[index].duration) {
			if (combo.steps[index].canCombo && stateMachine.attackQueue.Count > 0) {
				AttackType nextAttackType = stateMachine.attackQueue.Peek();
				ComboSequence nextCombo = GetComboBasedOnType(nextAttackType);
				int nextIndex = index + 1;
				
				if(nextIndex < nextCombo.steps.Length - 1) {
					stateMachine.SetNextState(new GroundComboState(stateMachine.attackQueue.Dequeue()));
				}else if(nextIndex == nextCombo.steps.Length - 1) {
					stateMachine.SetNextState(new GroundFinisherState(stateMachine.attackQueue.Dequeue()));
				} else {
					stateMachine.SetNextState(new IdleState());
				}
			} else {
				stateMachine.SetNextState(new IdleState());
			}
		}
	}
}