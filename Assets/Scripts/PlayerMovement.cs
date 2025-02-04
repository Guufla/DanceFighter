using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    Vector2 moveInput; 

    [SerializeField] float movementSpeed = 3f;

    [SerializeField] float jumpStrength = 4f;

    Rigidbody2D playerRigidbody;

    Boolean groundCheck;

    Boolean stopMovement;

    Boolean stopYMovement;

    float setGravityScale;


    // Start is called before the first frame update
    void Start()
    {
        playerRigidbody = GetComponent<Rigidbody2D>();
        setGravityScale = playerRigidbody.gravityScale;
    }

    // Update is called once per frame
    void Update()
    {
        getGroundCheck();
        getMovementInfo();
        if(stopMovement == false){
            movement();
        }
        if(stopYMovement){
            playerRigidbody.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
        }
        else{
            playerRigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

    }
    void getMovementInfo(){
        if(transform.tag == "Player1"){
            stopMovement = GameManager.Instance.stopP1Movement;
            stopYMovement = GameManager.Instance.stopP1YMovement;
        }
        else{
            stopMovement = GameManager.Instance.stopP2Movement;
            stopYMovement = GameManager.Instance.stopP2YMovement;
        }
    }

    void getGroundCheck(){
        if(transform.tag == "Player1"){
            groundCheck = GameManager.Instance.player1IsOnGround;
        }
        else{
            groundCheck = GameManager.Instance.player2IsOnGround;
        }
    }

    void OnMove(InputValue value){
        moveInput = value.Get<Vector2>(); // Gets the players movement input
    }

    void movement(){
        Vector2 playerVelocity = new Vector2(moveInput.x *  movementSpeed,playerRigidbody.velocity.y); // Only takes in the horizontal movement input
        playerRigidbody.velocity = playerVelocity; // The velocity of the rigid body is the players movement
    }
    void OnJump(InputValue value){
        if(value.isPressed && groundCheck){
            playerRigidbody.velocity += new Vector2(0f,jumpStrength); // Code to jump on pressing space
        }
        
    }
}
