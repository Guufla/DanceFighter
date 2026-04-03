using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBase : State
{
    protected Animator attackAnimator;
    public Collider2D[] attackBoxColliders;
    private List<Collider2D> collidersDamaged;
    protected float attackBuffer = 0;
    private bool isBufferingEnabled = false;
    protected Queue<AttackType> attackQueue = new Queue<AttackType>();
    public bool canChain;
    protected int attackIndex;
    protected AttackType curAttackType;
    protected ComboSequence lightCombo;
    protected ComboSequence mediumCombo;
    public float comboResetTimer;
    
    private PlayerInputHandler inputHandler;

    public override void OnEnter(StateMachine stateMachine)
    {
        base.OnEnter(stateMachine);
        attackAnimator = stateMachine.animator;
        ComboCheck comboCheck = stateMachine.GetComponent<ComboCheck>();
        inputHandler = comboCheck?.GetPlayerInputHandler();
        if(inputHandler != null)
        {
            inputHandler.OnAttackInput += OnAttackInput;
        }else {
            Debug.LogWarning("PlayerInputHandler not found on ComboCheck component.");
        }
        lightCombo = stateMachine.lightCombo;
        mediumCombo = stateMachine.mediumCombo;
        attackBoxColliders = stateMachine.attackBoxColliders;
        collidersDamaged = new List<Collider2D>();
    }
    
    protected void EnableBuffering()
    {
        stateMachine.isBuffering = true;
    }
    
    protected void DisableBuffering()
    {
        stateMachine.isBuffering = false;
    }
    
    public void OnAttackInput(AttackType attackType)
    {
        if(stateMachine.attackQueue.Count < 3 && stateMachine.isBuffering)
        {
            stateMachine.attackQueue.Enqueue(attackType);
            stateMachine.attackBuffer = 2f;
            Debug.Log("Attack queued: " + attackType);
        }
    }
    
    public override void OnUpdate(){
        base.OnUpdate();
        
        if (stateMachine.attackBuffer > 0)
        {
            stateMachine.attackBuffer -= Time.deltaTime;
            if (stateMachine.attackBuffer <= 0)
            {
                stateMachine.attackQueue.Clear();
                stateMachine.attackBuffer = 0;
                stateMachine.isBuffering = false;
            }
        }
    }
    
    public override void OnExit()
    {
        base.OnExit();
        if(inputHandler != null)
        {
            inputHandler.OnAttackInput -= OnAttackInput;
        }
    }
    
    protected ComboSequence GetLightCombo()
    {
        return lightCombo;
    }
    
    protected ComboSequence GetMediumCombo()
    {
        return mediumCombo;
    }
}
