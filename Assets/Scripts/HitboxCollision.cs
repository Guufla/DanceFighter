using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxCollision : MonoBehaviour
{
    // IMPORTANT: This is also attack behavior not just hitbox collision
    // By attack behavior I mean anything that is affected by the collision like health and knockback



    // Variables
    GameObject player;

    float basicAttackKnockBackX;

    float basicAttackKnockBackY;



    // All opponent variables
    GameObject oppositePlayer;
    Rigidbody2D oppositeRigidBody;

    float oppositeHealth;

    float facingX;

    float facingY;



    // Start is called before the first frame update
    void Start()
    {
        

        player = transform.parent.gameObject;


        // Makes sure the opposite player variable is correct
        if(player.CompareTag("Player1")){
            basicAttackKnockBackX = GameManager.Instance.P1AttackKnockBackX;

            basicAttackKnockBackY = GameManager.Instance.P1AttackKnockBackY;


            oppositePlayer = GameManager.Instance.player2;
        }
        else if(player.CompareTag("Player2")){
            basicAttackKnockBackX = GameManager.Instance.P2AttackKnockBackX;

            basicAttackKnockBackY = GameManager.Instance.P2AttackKnockBackY;

            oppositePlayer = GameManager.Instance.player1;
        }

        oppositeRigidBody = oppositePlayer.GetComponent<Rigidbody2D>();


        facingX = 0;

        facingY = 0;
    }

    void Update(){
        if(player.CompareTag("Player1")){
            basicAttackKnockBackX = GameManager.Instance.P1AttackKnockBackX;

            basicAttackKnockBackY = GameManager.Instance.P1AttackKnockBackY;
        }
        else if(player.CompareTag("Player2")){
            basicAttackKnockBackX = GameManager.Instance.P2AttackKnockBackX;

            basicAttackKnockBackY = GameManager.Instance.P2AttackKnockBackY;
        }

        
    }

    // Update is called once per frame
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag(oppositePlayer.tag)){
            
            facingX = player.transform.localScale.x;

            facingY = player.transform.localScale.y;

            // facing accounts for which way the player is facing

            // Needs to be more complex so that depending on where its hit from there will be a different knockback and stun
            oppositeRigidBody.velocity += new Vector2(basicAttackKnockBackX * facingX,basicAttackKnockBackY * facingY );

            Debug.Log("Hit");
        }


    }
}
