using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//commented out in PlayerAttack.cs
public enum AttackType : int{
        none,attackE, attackR, attackF, upTiltE, downTiltE, upTiltR, downTiltR, upTiltF, downTiltF
    }
public class ComboCheck : MonoBehaviour
{

    private StateMachine comboStateMachine;
	public AttackType currentAttackType = AttackType.none;
    [SerializeField] private int playerIndex;
    private PlayerInputHandler playerInputHandler;
    [SerializeField] private GameObject sprite;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private ComboSequence lightCombo;
    [SerializeField] private ComboSequence mediumCombo;
    [SerializeField] private Collider2D[] attackHitboxColliders;

    // Start is called before the first frame update
    void Awake()
    {
        comboStateMachine = GetComponent<StateMachine>();
        attackHitboxColliders = sprite.GetComponentsInChildren<Collider2D>();
        playerAnimator = sprite.GetComponent<Animator>();
        comboStateMachine.lightCombo = lightCombo;
        comboStateMachine.mediumCombo = mediumCombo;
        comboStateMachine.animator = playerAnimator;
        comboStateMachine.attackBoxColliders = attackHitboxColliders;
    }
    public void OnPlayerJoined(PlayerInputHandler inputHandler)
    {
        if(inputHandler.playerIndex == playerIndex)
        {
            playerInputHandler = inputHandler;
            inputHandler.OnAttackInput += UpdateAttackType;
            //Debug.Log("Subscribed to OnAttackInput for playerIndex: " + playerIndex);
        }
    }

    public PlayerInputHandler GetPlayerInputHandler()
    {
        return playerInputHandler;
    }
    public int GetPlayerIndex()
    {
        return playerIndex;
    }
    
    private void UpdateAttackType(AttackType type)
    {
        //Debug.Log("UpdateAttackType called with type: " + type);
        currentAttackType = type;
    }

    void Update()
    {
        //Debug.Log("Current Attack Type: " + currentAttackType);
        //Debug.Log(comboStateMachine.currentState.GetType() == typeof(IdleState));
        
        if (currentAttackType != AttackType.none && comboStateMachine.currentState.GetType() == typeof(IdleState))
        {
            comboStateMachine.SetNextState(new GroundEntryState(currentAttackType));
            currentAttackType = AttackType.none;
        }
    }
}