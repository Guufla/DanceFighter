using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

enum DefenseState: int { none,block, parry, guardBreak }

public class PlayerDefense : MonoBehaviour
{
    CapsuleCollider2D playerCollider;
    GameObject player;
    
    CapsuleCollider2D attackBoxCollider;
    
    public Vector3 blockCOlliderSizeMultiplier = new Vector3(1.5f, 1.5f, 1.3f); 
    private Vector3 originalColliderSize;
    Input inputActions; 
    
    DefenseState defenseType;

    [Header("Block/Parry")]
    public bool isBlocking;
    public bool isParrying;
    public float blockDamageReduction = 0.5f;
    public float parryTimeWindow = 0.2f;
    //private float parryTimer = 0;
    //private bool parryWindowActive = false;
    
    [Header("Guard Meter System")]
    public float maxGuardMeter = 100f;
    public float guardMeter;
    public float guardRegenRate = 10f;
    private bool isGuardBroken = false;
    
    private void Awake()
    {
        defenseType = DefenseState.none;
    
        playerCollider = GetComponentInChildren<CapsuleCollider2D>();
        attackBoxCollider = GetComponentInChildren<CapsuleCollider2D>();
        inputActions = new Input();
        guardMeter = maxGuardMeter;
        
        if(playerCollider != null)
        {
            originalColliderSize = playerCollider.transform.localScale;
        }
    }
    
    private void OnEnable() 
    {
        inputActions.Player.Block.started += OnBlockStarted;
        inputActions.Player.Block.performed += OnBlockPerformed;
        inputActions.Player.Block.canceled += OnBlockCanceled;
        
        inputActions.Player.Parry.performed += OnParryPerformed;
    
        inputActions.Enable();    
    }
    
    private void OnDisable()
    {
        inputActions.Disable();
    }
    
    void getPlayerBlocking()
    {
        if(transform.tag == "Player1")
        {
            player = GameManager.Instance.player1;
        }
        else if(transform.tag == "Player2")
        {
            player = GameManager.Instance.player2;
        }
    }
    
    private void OnBlockStarted(InputAction.CallbackContext context)
    {
        
        if(!isGuardBroken)
        {
            if (context.interaction is HoldInteraction)
            {
                isBlocking = true;
                defenseType = DefenseState.block;
                Debug.Log("Block Started");
            }
            
            if(playerCollider != null)
            {
                playerCollider.transform.localScale = Vector3.Scale(originalColliderSize, blockCOlliderSizeMultiplier);
            }
        }
            
    }
    
    private void OnBlockPerformed(InputAction.CallbackContext context)
    {
        if(!isGuardBroken)
        {
            if (context.interaction is HoldInteraction)
            {
                isBlocking = true;
                defenseType = DefenseState.block;
                Debug.Log("Block Performed");
            }
        }
    }
    
    private void OnBlockCanceled(InputAction.CallbackContext context)
    {
        if (context.interaction is HoldInteraction)
        {
            CancelBlock();
        }
    }
    
    private void CancelBlock()
    {
        isBlocking = false;
        defenseType = DefenseState.none;
        Debug.Log("Block Canceled");

        if (playerCollider != null)
        {
            playerCollider.transform.localScale = originalColliderSize;
        }
    }
    
    private void OnParryPerformed(InputAction.CallbackContext context)
    {
        if (context.interaction is TapInteraction)
        {
            if(context.duration <= parryTimeWindow)
            {
                isParrying = true;
                defenseType = DefenseState.parry;
                Debug.Log("Parry Performed");
                Invoke(nameof(ResetParry), parryTimeWindow); // Reset parry state after time window
            }else {
                Debug.Log("Parry Failed");
            }
        }
    }
    
    void ResetParry()
    {
        isParrying = false;
        defenseType = DefenseState.none;
    }
    
    private void Update()
    {
        
    }
    
    public void TakeDamage(float damage, Collider2D attackCollider)
    {
    
        if(attackBoxCollider.bounds.Intersects(attackCollider.bounds))
        {
            if(isBlocking)
            {
                Debug.Log("Blocked");
                // Apply block damage reduction
                float reducedDamage = damage * blockDamageReduction;
                guardMeter -= reducedDamage;
                
                getPlayerBlocking();
                
                Debug.Log("Player " + player.tag);
                
                if(player == GameManager.Instance.player1)
                {
                    GameManager.Instance.P1Health -= ((int)reducedDamage * 1/4);
                    Debug.Log("Player 1 Health: " + GameManager.Instance.P1Health);
                    GameManager.Instance.playerHealthBar.value = GameManager.Instance.P1Health;
                }
                else if(player == GameManager.Instance.player2)
                {
                    
                    GameManager.Instance.P2Health -= ((int)reducedDamage * 1/4);
                    GameManager.Instance.opponentHealth.value = GameManager.Instance.P2Health;
                    Debug.Log("Player 2 Health: " + GameManager.Instance.P2Health);
                }
                
                Debug.Log("Guard Meter: " + guardMeter);
                
                if(guardMeter <= 0 && !isGuardBroken) 
                {
                    isGuardBroken = true;
                    defenseType = DefenseState.guardBreak;
                    Debug.Log("Guard Broken");
                    
                    //stop blocking
                    CancelBlock();
                    //stop blocking even if w is held
                    StartCoroutine(GuardRegen()); 
                }
            }else if(isParrying)
            {
                Debug.Log("Parried");
                CancelBlock();
            }else{
                guardMeter -= damage;
                // Normal damage logic here
            }
        }
    
        IEnumerator GuardRegen()
        {
            while(isGuardBroken)
            {
                guardMeter += guardRegenRate * Time.deltaTime; //regen rate
                if(guardMeter >= maxGuardMeter)
                {
                    guardMeter = maxGuardMeter;
                    isGuardBroken = false;
                    defenseType = DefenseState.none;
                    Debug.Log("Guard Recovered");
                }
                
                yield return null;
            }
        }
    }
}
