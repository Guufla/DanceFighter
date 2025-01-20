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


    // Start is called before the first frame update
    void Start()
    {
        playerRigidbody = GetComponent<Rigidbody2D>();
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
        Vector2 playerVelocity = new Vector2(moveInput.x *  movementSpeed,playerRigidbody.velocity.y); // Only takes in the horizontal movement input
        playerRigidbody.velocity = playerVelocity; // The velocity of the rigid body is the players movement
    }
    void OnJump(InputValue value){
        if(value.isPressed){
            playerRigidbody.velocity += new Vector2(0f,jumpStrength); // Code to jump on pressing space
        }
        
    }
}
