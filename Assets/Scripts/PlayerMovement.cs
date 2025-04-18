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

    
    [SerializeField]float dashStrength = 0f;

    [SerializeField]float dashTimeVariable = 0f;

    Rigidbody2D playerRigidbody;

    Boolean groundCheck;

    Boolean stopMovement;

    Boolean stopYMovement;

    public Boolean dashOption;

    public Boolean isDashing;

    public Boolean isDashContinued;

    Boolean dashBufferIndicator;

    Boolean disablePlayerInput;

    float setGravityScale;
    
    Animator playerAnimator;
    PlayerDirections playerDirections;
    PlayerAttack playerAttack;
    
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
        playerDirections = GetComponent<PlayerDirections>();
        playerAttack = GetComponent<PlayerAttack>();
        dashOption = true;
        dashBufferIndicator = false;
        disablePlayerInput = false;
        isDashContinued = false;
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
        airDashCheck();
        isHit();
        handleJumping();
        if(stopMovement == false && isDashing == false && disablePlayerInput == false){
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
        disablePlayerInput = GameManager.Instance.disablePlayerInputs;

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
            dashOption = true;
        }
    }
    
    void isHit()
    {
        if(playerAnimator.GetBool("isHit") == false)
        {
            playerAnimator.SetLayerWeight(1, 0f);
        }else{
            playerAnimator.SetLayerWeight(1, 1f);
        }
    }
    
    void handleJumping()
    {
        // Check if the player is on the ground or performing an air attack
        if (transform.position.y == initialYPos || playerAttack.isAirAttacking == true)
        {
            playerAnimator.SetBool("isJumping", false);
            // Debug.Log(playerAttack.isAirAttacking);
        }
        else
        {
            // Check if the player is in the air and not performing an air attack
            if (transform.position.y > highestYPos && playerAttack.isAirAttacking == false)
            {
                highestYPos = transform.position.y;
                playerAnimator.SetBool("isJumping", true);
            }
        }
    }

    void movement(){
        // VERY IMPORTANT
        // ADD HITSTUN OR CHANGE HOW MOVEMENT WORKS SO THAT KNOCKBACK IS POSSIBLE
        
        
        //manipulates the layer weight based on if an animation is playing or not
        if(playerAnimator.GetFloat("WalkDirection") == 0 && 
        playerAnimator.GetBool("isJumping") == false && 
        playerAnimator.GetBool("isBlocking") == false &&
        playerAnimator.GetBool("isParrying") == false)
        {
            playerAnimator.SetLayerWeight(1, 0f);
        }else {
            playerAnimator.SetLayerWeight(1, 1f);
        }
        
        //velocity calculations
        Vector2 playerVelocity = new Vector2(moveInput.x * movementSpeed,playerRigidbody.velocity.y); // Only takes in the horizontal movement input
        if((gameObject.CompareTag("Player1") && GameManager.Instance.isOffensiveP1) || (gameObject.CompareTag("Player2") && GameManager.Instance.isOffensiveP2))
        {
            playerVelocity.x *= 1.5f;
        }
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
            
    }
    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && groundCheck && !disablePlayerInput)
        {
            playerRigidbody.velocity += new Vector2(0f, jumpStrength); // Code to jump on pressing space
        }
    }

    public void Dash(InputAction.CallbackContext context)
    {
        if (context.performed && dashOption && !isDashing && !disablePlayerInput && !isDashContinued)
        {
            //Debug.Log("Dash performed");
            dashBufferIndicator = false;
            isDashing = true;
            StartCoroutine(dashBuffer());
            playerRigidbody.AddForce(new Vector2(moveInput.x * dashStrength ,0f), ForceMode2D.Impulse);
            StartCoroutine(dashTime());
            dashOption = false;
        }
    }

    public void airDashCheck(){
        if(isDashContinued && groundCheck)
        {
            isDashing = false;
            isDashContinued = false;
        }
    }
    IEnumerator dashTime(){
        yield return new WaitForSeconds(dashTimeVariable);
        if(groundCheck)
        {
            isDashing = false;
        }
        else
        {
            isDashContinued = true;
        }
    }
    IEnumerator dashBuffer(){
        dashBufferIndicator = false;
        yield return new WaitForSeconds(0.3f);
        dashBufferIndicator = true;
    }
}
