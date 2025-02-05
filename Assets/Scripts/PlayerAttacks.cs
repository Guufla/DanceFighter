using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using UnityEngine.InputSystem;


// Once you get rid of the other script you can add this one in
// enum AttackType : int{
//         none,attackE, attackR, attackF
//     }

// enum Direction : int{
//         up,down
//     }


public class PlayerAttacks : MonoBehaviour
{

   Animator attackAnimator;

    [SerializeField] GameObject attackBoxObject;
    [SerializeField] GameObject attackBoxCollider;

    AttackType attackPressed;

    AttackType currentAttackPressed;

    public bool holdingUp;
    
    public bool holdingDown;

    public bool isOnGround;

    public bool canAttack;

    public bool canInput;

    public bool canAirCombo;

    public bool isAirAttacking;

    public bool currentlyAttacking;

    public bool comboResetTimerActive;

    [SerializeField] float comboResetTimer;

    [SerializeField] float coolDownTime = 2f;

    AnimatorStateInfo currentStateInfo;


    float knockbackX;

    float knockbackY;

    float curCombo;

    Boolean stopPlayerMovement; // For when an attack stops the player from making any more movements

    Boolean stopPlayerYMovement;

    Rigidbody2D playerRigidbody;
    
    void updateCanInput(){
        if(transform.tag == "Player1"){
            canInput = GameManager.Instance.canInputP1;
        }
        else{
            canInput = GameManager.Instance.canInputP2;
        }
    }
    


    // Start is called before the first frame update
    void Start()
    {
        attackPressed = AttackType.none; // Used to signify which attack button is pressed
        holdingUp = false;
        holdingDown = false;
        isOnGround = false;
        

        // Would need to change the code surrounding this if I want to use multiple hitboxes
        attackAnimator = attackBoxObject.GetComponent<Animator>(); // Reference to the attack hitbox animator


        canAttack = true; // Determines if the player can attack or not
        canAirCombo = true;
        canInput = true;
        currentlyAttacking = false;
        comboResetTimerActive = false; // used for the reset combo timer (Explained more near that section)
        comboResetTimer = 0;


        knockbackX = 0; 
        knockbackY = 0;

        curCombo = 0; // Keeps track of the current combo. Used to move from one attack to the next like light attack 1 to light attack 2

        stopPlayerMovement = false;
        stopPlayerYMovement = false;
        
        playerRigidbody = transform.GetComponent<Rigidbody2D>();

    }

    // Update is called once per frame
    void Update()
    {
        attackColliderUpdate(); // Update function for the attack collider
        groundCheck();
        newAttacks(); // Update function for the attack system
        resetTimerCheck(); // Update function for resetting the combo timer
        updateMovement(); // If the player is to have their movement stopped it is sent to here

        Debug.Log(canInput);
    }

    

    
    void attackColliderUpdate(){

        // Determines if the attack hitbox object should be enabled or disabled. In the actual game there wont be a sprite so its like enabling and disabling the hitbox itself
        if(currentAttackPressed == AttackType.none){
            attackBoxCollider.SetActive(false);
        }
        else{
            attackBoxCollider.SetActive(true);
        }
    }

    void groundCheck(){
        if(transform.tag == "Player1"){
            isOnGround = GameManager.Instance.player1IsOnGround;
        }
        else{
            isOnGround = GameManager.Instance.player2IsOnGround;
        }

        if(isOnGround){
            canAirCombo = true;
        }
    }

    void stopMovement(Boolean stopPlayerMovement, Boolean stopPlayerYMovement){
        this.stopPlayerMovement = stopPlayerMovement; 
        this.stopPlayerYMovement = stopPlayerYMovement;
    }   

    // This is the new attack system I setup, old one is commented out for now but this one sets up the next attack based on the button pressed during the combo animation.
    void newAttacks()
    {
        currentStateInfo = attackAnimator.GetCurrentAnimatorStateInfo(0); // Gets the current state of the animator

        updateCanInput();
        // When you are able to attack it records the key you press
        // if(canAttack == true){
        //     currentAttackPressed = attackPressed;
        //     attackPressed = AttackType.none; // Resets the attack pressed for the next loop
        // }

        if(currentAttackPressed == AttackType.attackE){
            
            if(curCombo > 2 || curCombo == 0){
                stopMovement(true,true);
                hitboxAnimation(1,"EAttack1","LightAttack1", false, 1.5f,0f,2f,0f);
            }
            else if(curCombo == 1){
                stopMovement(true,true);
                hitboxAnimation(1,"EAttack2","LightAttack2", false, 1.5f,0f,2f,0f);
            }
            else if(curCombo == 2){
                stopMovement(true,true);
                hitboxAnimation(1,"EAttack3","LightAttack3", false, 1.5f,0f,2f,0f);
            }


            // Updates the knockback variables in the game instance

            if(gameObject.CompareTag("Player1"))
            {
                GameManager.Instance.P1AttackKnockBackX = knockbackX;
                GameManager.Instance.P1AttackKnockBackY = knockbackY;
            }
            else if (gameObject.CompareTag("Player2")) 
            {
                GameManager.Instance.P2AttackKnockBackX = knockbackX;
                GameManager.Instance.P2AttackKnockBackY = knockbackY;
            }
            else
            {
                Debug.Log("Error assigning knockback to players");
            }

        }

        if (currentAttackPressed == AttackType.attackR)
        {
            

            if(gameObject.CompareTag("Player1")){
                GameManager.Instance.P1AttackKnockBackX = knockbackX;
                GameManager.Instance.P1AttackKnockBackY = knockbackY;
            }
            else if (gameObject.CompareTag("Player2")) {
                GameManager.Instance.P2AttackKnockBackX = knockbackX;
                GameManager.Instance.P2AttackKnockBackY = knockbackY;
            }
            else{
                Debug.Log("Error assigning knockback to players");
            }
        }

        if (currentAttackPressed == AttackType.attackF)
        {

            

            if(gameObject.CompareTag("Player1")){
                GameManager.Instance.P1AttackKnockBackX = knockbackX;
                GameManager.Instance.P1AttackKnockBackY = knockbackY;
            }
            else if (gameObject.CompareTag("Player2")) {
                GameManager.Instance.P2AttackKnockBackX = knockbackX;
                GameManager.Instance.P2AttackKnockBackY = knockbackY;
            }
            else{
                Debug.Log("Error assigning knockback to players");
            }
    }
    void hitboxAnimation(int version, String animatorBool, String animationName, Boolean isAirAttacking, float knockBackX, float knockBackY, float dashX, float dashY){

        
    }


    }

    void resetKnockback(){
        setKnockback(0f,0f); // Reset knockback
    }


    // Create more In the input manager with a different letter to indicate the different buttons used
    void OnAttackE(InputValue value)
    {
        if(canInput){
            attackPressed = AttackType.attackE;
        }
    }

    void OnAttackR(InputValue value)
    {
        if(canInput){
            attackPressed = AttackType.attackR;
        }
    }

    void OnAttackF(InputValue value)
    {
        if(canInput){
            attackPressed = AttackType.attackF;
        }
    }

    void OnUpPressed(InputValue value){
        float inputValue = value.Get<float>();

        if (inputValue > 0)
        {
            holdingUp = true;
        }
        else
        {
            holdingUp = false;
        }
    }

     void OnDownPressed(InputValue value){
        float inputValue = value.Get<float>();

        if (inputValue > 0)
        {
            holdingDown = true;
        }
        else
        {
            holdingDown = false;
        }
    }

    void updateMovement(){
        if(transform.tag == "Player1"){
            GameManager.Instance.stopP1Movement = stopPlayerMovement;
            GameManager.Instance.stopP1YMovement = stopPlayerYMovement;
        }
        else{
            GameManager.Instance.stopP2Movement = stopPlayerMovement;
            GameManager.Instance.stopP2YMovement = stopPlayerYMovement;
        }
    }

    
    // When the player sits on a combo for too long without pressing anything the combo resets to zero


    /* This code is meant to act as a combo reset, when the player is on the 1st attack and doesnt press anything within a certain time then the combo resets to 0 */
    void resetTimerCheck(){ 
        if(comboResetTimerActive){
            if(comboResetTimer > 0){
                comboResetTimer -= Time.deltaTime;
            }
            else{
                resetAttacks();
                comboResetTimerActive = false;
            }
        }
    }

    // Resets attack state to idle
    void resetAttacks(){
        

        
    }


    // Quick function for setting the knockback variables
    void setKnockback(float x , float y){
        knockbackX = x;
        knockbackY = y;
    }

    void dashWithAttack(float x, float y){
        playerRigidbody.velocity = new Vector2(x * transform.localScale.x,y); // Code to jump on pressing space
    }

    // Coroutine that is currently unused
    private IEnumerator attackingCooldown(){
        yield return new WaitForSeconds(coolDownTime);
        resetAttacks();
        canAttack = true;
    }




}



