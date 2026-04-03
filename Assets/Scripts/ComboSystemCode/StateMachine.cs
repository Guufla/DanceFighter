using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
	public State currentState { get; private set; }
	private State nextState;
	public Animator animator;
	public Collider2D[] attackBoxColliders;
	public ComboSequence lightCombo;
	public ComboSequence mediumCombo;
	public Queue<AttackType> attackQueue = new Queue<AttackType>();
	public int attackIndex;
	public float attackBuffer = 0f;
	public bool isBuffering = false;

	private void Awake()
	{
		currentState = new IdleState();
		currentState.OnEnter(this);
	}
	
	void Update()
	{
		if(nextState != null)
		{
			SetState(nextState);
		}
	
		if(currentState != null)
		{
			currentState.OnUpdate();
		}
	}
	
	private void SetState(State newState)
	{
		nextState = null;
		if(currentState != null)
		{
			currentState.OnExit();
		}
		
		currentState = newState;
		if(currentState != null)
		{
			currentState.OnEnter(this);
		}		
	}
	
	public void SetNextState(State newState)
	{
		if(newState != null)
		{
			nextState = newState;
		}
	}

	private void FixedUpdate()
	{
		if(currentState != null)
		{
			currentState.OnFixedUpdate();
		}
	}
}