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
    
    [SerializeField] private GameObject playerSprite;
    
    public Animator playerAnimator;
    CapsuleCollider2D attackBoxCollider;
    
    public Vector3 blockCOlliderSizeMultiplier = new Vector3(1.5f, 1.5f, 1.3f); 
    private Vector3 originalColliderSize;
    Input inputActions; 
    
    DefenseState defenseType;

    [Header("Block/Parry")]
    private float blockDamageReduction = 0.5f;
    private float parryTimeWindow = 0.2f;
    
    [Header("Guard Meter System")]
    private float maxGuardMeter = 100f;
    private float guardMeter;
    private float guardRegenRate = 20f;    
    private void Awake()
    {
        playerAnimator = playerSprite.GetComponent<Animator>();
        defenseType = DefenseState.none;
        
        Transform playerColliderTransform = transform.Find("PlayerCollider"); // Replace "PlayerCollider" with the actual name of the child
        if (playerColliderTransform != null)
        {
            playerCollider = playerColliderTransform.GetComponent<CapsuleCollider2D>();
        }
    
        Transform attackBoxColliderTransform = transform.Find("AttackHitbox"); // Replace "AttackBoxCollider" with the actual name of the child
        if (attackBoxColliderTransform != null)
        {
            attackBoxCollider = attackBoxColliderTransform.GetComponentInChildren<CapsuleCollider2D>();
        }
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
        
        if(!playerAnimator.GetBool("isGuardBroken"))
        {
            if (context.interaction is HoldInteraction)
            {
                //playerAnimator.SetBool("isBlocking", true);
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
        if(!playerAnimator.GetBool("isGuardBroken"))
        {
            if (context.interaction is HoldInteraction)
            {
                playerAnimator.SetBool("isBlocking", true);
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
        playerAnimator.SetBool("isBlocking", false);
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
                playerAnimator.SetBool("isParrying", true);
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
        playerAnimator.SetBool("isParrying", false);
        playerAnimator.SetBool("isBlocking", false);
        defenseType = DefenseState.none;
    }
    
    private void Update()
    {
        
    }
    
    public void TakeDamage(float damage, Collider2D attackCollider)
    {
        if(attackBoxCollider.bounds.Intersects(attackCollider.bounds))
        {
            if(playerAnimator.GetBool("isBlocking"))
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
                
                if(guardMeter <= 0 && !playerAnimator.GetBool("isGuardBroken")) 
                {
                    playerAnimator.SetBool("isBlocking", false);
                    playerAnimator.SetBool("isGuardBroken", true);
                    defenseType = DefenseState.guardBreak;
                    Debug.Log("Guard Broken");
                    
                    //stop blocking
                    CancelBlock();
                    //stop blocking even if w is held
                    StartCoroutine(GuardRegen()); 
                }
            }else if(playerAnimator.GetBool("isParrying")) {
                Debug.Log("Parried");
                CancelBlock();
            }else{
                guardMeter -= damage;
                //meant to do more here but forgot what
            }
        }
    
        IEnumerator GuardRegen()
        {
            while(playerAnimator.GetBool("isGuardBroken"))
            {
                guardMeter += guardRegenRate * Time.deltaTime; //regen rate
                if(guardMeter >= maxGuardMeter)
                {
                    guardMeter = maxGuardMeter;
                    playerAnimator.SetBool("isGuardBroken", false);
                    defenseType = DefenseState.none;
                    Debug.Log("Guard Recovered");
                }
                
                yield return null;
            }
        }
    }
}
