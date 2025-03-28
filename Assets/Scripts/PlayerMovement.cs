using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    Vector2 moveInput;     
    Input inputActions;
    [SerializeField] float movementSpeed = 3f;

    [SerializeField] float jumpStrength = 4f;

    Rigidbody2D playerRigidbody;

    Boolean groundCheck;

    Boolean stopMovement;

    Boolean stopYMovement;

    float setGravityScale;
    
    Animator playerAnimator;
    PlayerDirections playerDirections;
    
    [SerializeField] private GameObject playerSprite;
    private float initialYPos;
    private float highestYPos;

    [SerializeField] private int playerIndex = 0;

    void Awake()
    {
        // inputActions = new Input();
        // inputActions.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>(); // Reads the input value of the player which is -1,0,1 for left, no input, right
        // inputActions.Player.Move.canceled += ctx => moveInput = Vector2.zero;
        // inputActions.Enable();
    }
    // Start is called before the first frame update
    void Start()
    {
        playerRigidbody = GetComponent<Rigidbody2D>();
        setGravityScale = playerRigidbody.gravityScale;
        playerAnimator = playerSprite.GetComponent<Animator>();
        playerDirections = playerSprite.transform.parent.GetComponent<PlayerDirections>();
    }
    public void OnMovement(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            moveInput = context.ReadValue<Vector2>();
        }
        else if(context.canceled)
        {
            moveInput = Vector2.zero;
        }
        
    }

    public int GetPlayerIndex(){
        return playerIndex;
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
        
        if(groundCheck){ //gets the initial and highest y position of the player every time the player is on the ground
            initialYPos = transform.position.y;
            highestYPos = transform.position.y;
        }
    }

    void movement(){
        // VERY IMPORTANT
        // ADD HITSTUN OR CHANGE HOW MOVEMENT WORKS SO THAT KNOCKBACK IS POSSIBLE
        
        //manipulates the layer weight based on if an animation is playing or not
        if(playerAnimator.GetFloat("WalkDirection") == 0 && playerAnimator.GetFloat("JumpParameter") == 0 && playerAnimator.GetBool("isHit") == false)
        {
            playerAnimator.SetLayerWeight(1, 0f);
        }else {
            playerAnimator.SetLayerWeight(1, 1f);
        }
        
        Vector2 playerVelocity = new Vector2(moveInput.x * movementSpeed,playerRigidbody.velocity.y); // Only takes in the horizontal movement input
        playerRigidbody.velocity = playerVelocity; // The velocity of the rigid body is the players movement
        
        if(playerDirections.isFacingRight){
            if(moveInput.x != 0){ // If the player is moving
            playerAnimator.SetBool("isMoving",true);
            playerAnimator.SetFloat("WalkDirection", moveInput.x); //decides the direction of the player
            } else{
                playerAnimator.SetBool("isMoving",false);
                playerAnimator.SetFloat("WalkDirection", 0);
            }
        }else if(playerDirections.isFacingLeft){
            if(moveInput.x != 0){
                float actualMoveInput = -moveInput.x; //flips the input so that the player moves in the correct direction
                playerAnimator.SetBool("isMoving",true);
                playerAnimator.SetFloat("WalkDirection", actualMoveInput);
            } else{
                playerAnimator.SetBool("isMoving",false);
                playerAnimator.SetFloat("WalkDirection", 0);
            }
        }
        
        
        if(transform.position.y == initialYPos){ //if the player is on the ground
            playerAnimator.SetBool("isJumping",false);
            playerAnimator.SetFloat("JumpParameter", 0);
        }
        else{
            
            if(transform.position.y > highestYPos){
                highestYPos = transform.position.y;
                playerAnimator.SetBool("isJumping",true);
                playerAnimator.SetFloat("JumpParameter", 1); //is jumping
            }else if(transform.position.y < highestYPos)
            {
                playerAnimator.SetBool("isJumping",false);
                playerAnimator.SetFloat("JumpParameter", -1); //is falling
            }
        } 
    }
    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && groundCheck)
        {
            playerRigidbody.velocity += new Vector2(0f, jumpStrength); // Code to jump on pressing space
        }
    }
}
