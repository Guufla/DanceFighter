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


    // Start is called before the first frame update
    void Start()
    {
        playerRigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        setGroundCheck();
        movement();

    }

    void setGroundCheck(){
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
