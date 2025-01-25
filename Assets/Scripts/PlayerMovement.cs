using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    Vector2 moveInput; 

    [SerializeField] float movementSpeed = 3f;

    [SerializeField] float jumpStrength = 4f;

    Rigidbody2D playerRigidbody;

    private bool player1; //bool for being player1


    // Start is called before the first frame update
    void Start()
    {
        playerRigidbody = GetComponent<Rigidbody2D>();

        if(gameObject.tag == "Player1")
        {
            player1 = true;
        }
        else
        {
            player1 = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        movement();
        
    }

    void OnMove(InputValue value){
        moveInput = value.Get<Vector2>(); // Gets the players movement input
    }

    void movement(){
        if(player1)
        {
            if(GameManager.Instance.P1frozen == true)
            {
                return;
            }
        }
        else
        {
            if(GameManager.Instance.P2frozen == true)
            {
                return;
            }
        }

        Vector2 playerVelocity = new Vector2(moveInput.x *  movementSpeed,playerRigidbody.velocity.y); // Only takes in the horizontal movement input
        playerRigidbody.velocity = playerVelocity; // The velocity of the rigid body is the players movement
    }
    void OnJump(InputValue value){
        if(player1)
        {
            if(GameManager.Instance.P1frozen == true)
            {
                return;
            }
        }
        else
        {
            if(GameManager.Instance.P2frozen == true)
            {
                return;
            }
        }
        
        if(value.isPressed){
            playerRigidbody.velocity += new Vector2(0f,jumpStrength); // Code to jump on pressing space
        }
        
    }
}
