
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HitboxCollision : MonoBehaviour
{
    // IMPORTANT: This is also attack behavior not just hitbox collision
    // By attack behavior I mean anything that is affected by the collision like health and knockback



    // Variables
    
    private GameObject player;
    
    PlayerDefense playerDef;
    float basicAttackKnockBackX;

    float basicAttackKnockBackY;

    //public float playerHealth = 100;

    //public Slider healthbar;
    // All opponent variables


    float facingX;

    float facingY;

    
    private GameObject oppositePlayer;
    private Animator oppositePlayerAnimator;
    private Rigidbody2D oppositeRigidBody;


    private bool hasCollided = false;
    private float hitState = 0f;
    private PlayerAttack playerAttack;
    void Start()
    {


        player = transform.parent.parent.gameObject; // Gets the player object (Can also get it from the game manager but i thought this was better)


        //healthbar.value = oppositeHealth; // Sets the value of the health bar to the player health


        if (player.CompareTag("Player1"))
        {
            // Takes the attack knockback variable that is set in the player attack script and creates variables for them to be used later 
            basicAttackKnockBackX = 0;
            basicAttackKnockBackY = 0;


            player = transform.parent.parent.gameObject;
            

            basicAttackKnockBackX = GameManager.Instance.P1AttackKnockBackX;

            basicAttackKnockBackY = GameManager.Instance.P1AttackKnockBackY;


            oppositePlayer = GameManager.Instance.player2;

            oppositePlayerAnimator = oppositePlayer.GetComponentInChildren<Animator>();
            // Makes sure the opposite player variable is correct
            
        }

        else if (player.CompareTag("Player2"))
        {
            // Takes the attack knockback variable that is set in the player attack script and creates variables for them to be used later 
            basicAttackKnockBackX = 0;
            basicAttackKnockBackY = 0;


            player = transform.parent.parent.gameObject;

            basicAttackKnockBackX = GameManager.Instance.P2AttackKnockBackX;

            basicAttackKnockBackY = GameManager.Instance.P2AttackKnockBackY;

            oppositePlayer = GameManager.Instance.player1;
            
            oppositePlayerAnimator = oppositePlayer.GetComponentInChildren<Animator>();
        }
        
        playerAttack = player.GetComponent<PlayerAttack>();

     oppositeRigidBody = oppositePlayer.GetComponent<Rigidbody2D>();


     facingX = 0;

     facingY = 0;
    }


    void Update()

    {
        if (player.CompareTag("Player1"))
        {
            basicAttackKnockBackX = GameManager.Instance.P1AttackKnockBackX;

            basicAttackKnockBackY = GameManager.Instance.P1AttackKnockBackY;

            
        }
        else if (player.CompareTag("Player2"))
        {
            basicAttackKnockBackX = GameManager.Instance.P2AttackKnockBackX;

            basicAttackKnockBackY = GameManager.Instance.P2AttackKnockBackY;
        }
        
        if(!playerAttack.isAttacking && hasCollided)
        {
            hasCollided = false;
        }

    }
    
    private bool IsAnyAttackAnimationActive()
    {
        return playerAttack.attackAnimator.GetBool("EAttack1") ||
            playerAttack.attackAnimator.GetBool("EAttack2") ||
            playerAttack.attackAnimator.GetBool("EAttack3") ||
            playerAttack.attackAnimator.GetBool("RAttack1") ||
            playerAttack.attackAnimator.GetBool("RAttack2") ||
            playerAttack.attackAnimator.GetBool("RAttack3") ||
            playerAttack.attackAnimator.GetBool("RAttack4") ||
            playerAttack.attackAnimator.GetBool("RAttack5") ||
            playerAttack.attackAnimator.GetBool("FAttack1") ||
            playerAttack.attackAnimator.GetBool("FAttack2") ||
            playerAttack.attackAnimator.GetBool("FAttack3") ||
            playerAttack.attackAnimator.GetBool("FAttack4") ||
            playerAttack.attackAnimator.GetBool("AAttack1") ||
            playerAttack.attackAnimator.GetBool("AAttack2") ||
            playerAttack.attackAnimator.GetBool("AAttack3") ||
            playerAttack.attackAnimator.GetBool("AAttack4") ||
            playerAttack.attackAnimator.GetBool("AirDownTilt") ||
            playerAttack.attackAnimator.GetBool("AirUpTilt") ||
            playerAttack.attackAnimator.GetBool("UpTilt") ||
            playerAttack.attackAnimator.GetBool("DownTilt");
    }

    // Update is called once per frame
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (GameManager.Instance.isCountingDown)
        {
            //Debug.Log("Game is counting down, no hitbox collision");
            return;
        }
        
        if(!playerAttack.isAttacking)
        {
            //Debug.Log("Player is not attacking, no hitbox collision");
            return;
        }
        
        if(hasCollided)
        {
            //Debug.Log("Hitbox has already collided, no hitbox collision");
            return;
        }
        
        if (!IsAnyAttackAnimationActive())
        {
            //Debug.Log("No attack animation is active, no hitbox collision");
            return;
        }
        
        if (other.CompareTag(oppositePlayer.tag)) 
        {

            facingX = player.transform.localScale.x;

            facingY = player.transform.localScale.y;

            // facing accounts for which way the player is facing


            // Needs to be more complex so that depending on where its hit from there will be a different knockback and stun
            // Velocity of the knockback is determined by the direction facing times the knockback variable. This is done for x and y
            
            StartCoroutine(Knockback());

            hasCollided = true;
            playerDef = oppositePlayer.GetComponent<PlayerDefense>();
            
            if(playerDef.playerAnimator.GetBool("isBlocking") || playerDef.playerAnimator.GetBool("isParrying")){
                playerDef.TakeDamage(50, other);
                return;
            }
        
            oppositePlayerAnimator.SetBool("isHit", true); // Sets the isHit variable in the opposite player's animator to true
            if(hitState == 0f){ 
                hitState = 1f;
            }
            else{
                hitState = 0f;
            }
            oppositePlayerAnimator.SetFloat("HitStates", hitState);
            Invoke("ResetIsHit", 0.5f); // Resets the isHit variable in the opposite player's animator to false after 0.5 seconds
    }
    

        if (player.CompareTag("Player1") && other.CompareTag("Player2")) 
        {
                GameManager.Instance.Player1HitsPlayer2();
                //Debug.Log("Player 1 hits Player 2");
            
        }
    
        if (player.CompareTag("Player2") && other.CompareTag("Player1"))
        {
                GameManager.Instance.Player2HitsPlayer1();
                //Debug.Log("Player 2 hits Player 1");
        }

        
    
    }

    private IEnumerator Knockback()
    {
        yield return new WaitForSeconds(0.1f);

        Debug.Log("Hit");
        oppositeRigidBody.velocity += new Vector2(basicAttackKnockBackX * facingX * oppositeRigidBody.gravityScale,basicAttackKnockBackY * facingY * oppositeRigidBody.gravityScale);

    
    private void ResetIsHit()
    {
        if (oppositePlayerAnimator != null)
        {
            oppositePlayerAnimator.SetBool("isHit", false);
        }
        else
        {
            Debug.LogError("oppositePlayerAnimator is null in ResetIsHit");
        }
    }
}
