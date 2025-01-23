using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HitboxCollision : MonoBehaviour
{
    // IMPORTANT: This is also attack behavior not just hitbox collision
    // By attack behavior I mean anything that is affected by the collision like health and knockback



    // Variables
    GameObject player;

    float basicAttackKnockBackX;

    float basicAttackKnockBackY;



    //public float playerHealth = 100;

    //public Slider healthbar;
    // All opponent variables


    float facingX;

    float facingY;

    private GameObject oppositePlayer;
    private Rigidbody2D oppositeRigidBody;
    public float oppositeHealth = 100;  // opponents health variable




    // Start is called before the first frame update
    void Start()
    {


        player = transform.parent.gameObject; // Gets the player object (Can also get it from the game manager but i thought this was better)

        //healthbar.value = oppositeHealth; // Sets the value of the health bar to the player health


        if (player.CompareTag("Player1"))
        {
            // Takes the attack knockback variable that is set in the player attack script and creates variables for them to be used later 
            basicAttackKnockBackX = 0;
            basicAttackKnockBackY = 0;


            player = transform.parent.gameObject;


            // Makes sure the opposite player variable is correct
            if (player.CompareTag("Player1"))
            {
                basicAttackKnockBackX = GameManager.Instance.P1AttackKnockBackX;

                basicAttackKnockBackY = GameManager.Instance.P1AttackKnockBackY;


                oppositePlayer = GameManager.Instance.player2;
            }
            else if (player.CompareTag("Player2"))
            {
                basicAttackKnockBackX = GameManager.Instance.P2AttackKnockBackX;

                basicAttackKnockBackY = GameManager.Instance.P2AttackKnockBackY;

                oppositePlayer = GameManager.Instance.player1;
            }

            oppositeRigidBody = oppositePlayer.GetComponent<Rigidbody2D>();


            facingX = 0;

            facingY = 0;
        }
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


    }

    // Update is called once per frame
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(oppositePlayer.tag))
        {

            facingX = player.transform.localScale.x;

            facingY = player.transform.localScale.y;

            // facing accounts for which way the player is facing


            // Needs to be more complex so that depending on where its hit from there will be a different knockback and stun
            oppositeRigidBody.velocity += new Vector2(basicAttackKnockBackX * facingX, basicAttackKnockBackY * facingY);

            // Velocity of the knockback is determined by the direction facing times the knockback variable. This is done for x and y
            oppositeRigidBody.velocity += new Vector2(basicAttackKnockBackX * facingX, basicAttackKnockBackY * facingY);

            // When the player is hit they lose a certain amount of health 
            //oppositeHealth -= 10;
            //healthbar.value = oppositeHealth; // Updates the health bar to the new health value
            //if (oppositeHealth <= 0)
            //{
                //Debug.Log("Player is dead");
            //}
            // We need to adjust this later to be more dynamic


        }


    }
}
