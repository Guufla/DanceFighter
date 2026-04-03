using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State
{
	public StateMachine stateMachine;
	protected float fixedTime {get; set;}
	protected float time { get; set; }
    public virtual void OnEnter(StateMachine assignedStateMachine) 
    {
		  stateMachine = assignedStateMachine;
		  fixedTime = 0f;
		  time = 0f;
    }
    
    public virtual void OnUpdate() {
      time += Time.deltaTime;
    }
    public virtual void OnFixedUpdate() {
      fixedTime += Time.fixedDeltaTime;
    }
    public virtual void OnExit() { }
    
    
}