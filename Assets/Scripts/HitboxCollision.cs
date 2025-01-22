using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxCollision : MonoBehaviour
{

    // Variables
    private GameObject player;

    private float basicAttackKnockBackX;

    private float basicAttackKnockBackY;
    
    private bool hasCollided = false;

    // All opponent variables
    private GameObject oppositePlayer;
    private Rigidbody2D oppositeRigidBody;

    private float oppositeHealth; // opponents health variable

    // These are used 
    private float facingX; // either 1 or -1 and represents if you are facing forwards or backwards

    private float facingY; // either 1 or -1 and represents if you are facing up or down (I doubt this will be used very much)

    // Start is called before the first frame update
    void Start()
    {

        player = transform.parent.gameObject; // Gets the player object (Can also get it from the game manager but i thought this was better)

        
        if(player.CompareTag("Player1"))
        {
            // Takes the attack knockback variable that is set in the player attack script and creates variables for them to be used later 
            basicAttackKnockBackX = 0; 
            basicAttackKnockBackY = 0;

            oppositePlayer = GameManager.Instance.player2; // Set the opposite player. If we are tagged player 1 we want the opposite player to be the player 2 object.
        }
        else if(player.CompareTag("Player2"))
        {
            // Takes the attack knockback variable that is set in the player attack script and creates variables for them to be used later (This may not be needed since its also done in update)
            basicAttackKnockBackX = 0;
            basicAttackKnockBackY = 0;

            oppositePlayer = GameManager.Instance.player1; // Set the opposite player. If we are tagged player 2 we want the opposite player to be the player 1 object.
        }

        oppositeRigidBody = oppositePlayer.GetComponent<Rigidbody2D>();

        // Initializes facing to 0 (Might not be necassary)
        facingX = 0; 
        facingY = 0;
    }

    void Update(){

        // Every frame it updates the knockback variables in anticipation for an on trigger enter.
        if(player.CompareTag("Player1"))
        {
            basicAttackKnockBackX = GameManager.Instance.P1AttackKnockBackX;
            basicAttackKnockBackY = GameManager.Instance.P1AttackKnockBackY;
        }
        else if(player.CompareTag("Player2"))
        {
            basicAttackKnockBackX = GameManager.Instance.P2AttackKnockBackX;
            basicAttackKnockBackY = GameManager.Instance.P2AttackKnockBackY;
        }

        
    }

    // Update is called once per frame
    private void OnTriggerEnter2D(Collider2D other)
    {
    
        if(hasCollided){
            hasCollided = false;
            return;
        }
        if(other.CompareTag(oppositePlayer.tag))
        {
            // Updates the variables with the value representing where the player if facing
            facingX = player.transform.localScale.x;
            facingY = player.transform.localScale.y;

            // Velocity of the knockback is determined by the direction facing times the knockback variable. This is done for x and y
            oppositeRigidBody.velocity += new Vector2(basicAttackKnockBackX * facingX,basicAttackKnockBackY * facingY );

            Debug.Log("Hit");
            hasCollided = true;
        }


    }
    
}
