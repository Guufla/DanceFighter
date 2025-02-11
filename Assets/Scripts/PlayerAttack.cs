using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using UnityEngine.InputSystem;

enum AttackType : int{
        none,attackE, attackR, attackF, upTiltE, downTiltE
    }

enum Direction : int{
        up,down
    }

public class PlayerAttack : MonoBehaviour
{
    Animator attackAnimator;

    [SerializeField]GameObject attackBoxObject;

    [SerializeField] GameObject attackBoxCollider;

    AttackType attackPressed;

    AttackType currentAttackPressed;

    public bool holdingUp;
    
    public bool holdingDown;

    public bool isOnGround;

    public bool canAttack;

    public bool canInput;

    public bool isAnimating;

    public bool canAirCombo;

    public bool isAirAttacking;

    public bool isAttacking;

    public bool isTiltAttacking;

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
        isAttacking = false;
        isTiltAttacking = false;
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

        

    }

    

    
    void attackColliderUpdate(){

        // Determines if the attack hitbox object should be enabled or disabled. In the actual game there wont be a sprite so its like enabling and disabling the hitbox itself
        if(isAttacking == false){
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
    void updateInput(){
        if(transform.tag == "Player1"){
            canInput = GameManager.Instance.canInputP1;
            isAnimating = GameManager.Instance.isHitBoxAnimatingP1;
        }
        else{
            canInput = GameManager.Instance.canInputP2;
            isAnimating = GameManager.Instance.isHitBoxAnimatingP2;
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

        updateInput();
        // When you are able to attack it records the key you press
        if(canAttack == true){
            currentAttackPressed = attackPressed;
            attackPressed = AttackType.none; // Resets the attack pressed for the next loop

            if(currentAttackPressed != AttackType.none){
                canAttack = false; // Sets canAttack to false to prevent a backlog of attacks from building up
            }
        }

        if(currentAttackPressed == AttackType.upTiltE){


            
            stopMovement(true, true);
            hitboxAnimation(4,"UpTilt","UpTilt", false, 1.5f,0f,0f,0f);


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

        if(currentAttackPressed == AttackType.downTiltE){


            
            stopMovement(true, true);
            hitboxAnimation(4,"DownTilt","DownTilt", false, 1.5f,0f,0f,0f);
            

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

        if(currentAttackPressed == AttackType.attackE){
            
            
            
            // These can be switched into certain attacks 

            // if(isAirAttacking == true || !isOnGround ){
            //     if(isOnGround){
            //         if(currentStateInfo.normalizedTime >= 1 ){
            //             canAttack = true;
            //             stopPlayerMovement = false;
            //             stopPlayerYMovement = false;
            //             isAttacking = false;
            //             this.isAirAttacking = false;


            //             curCombo = 1; // For the sake of linking an attack that has a higher combo count than this one it has to set the combo back to 1
            //             resetAirAttacks();
            //             resetAttacks();
            //             resetKnockback(); // Resets the knockback stats for this attack

            //             isAirAttacking = false;
            //         }
            //         resetAirAttacks();
            //     }

            //     else if(attackAnimator.GetBool("AirUpTilt") && isAttacking == false || holdingUp && isAttacking == false )
            //     {
            //         stopMovement(false, false);
            //         hitboxAnimation(4,"AirUpTilt","airUpTilt", true, 1.5f,0f,0f,0f);
            //     }
            //     else if(attackAnimator.GetBool("AirDownTilt") && isAttacking == false || holdingDown && isAttacking == false )
            //     {
            //         stopMovement(false, false);
            //         hitboxAnimation(4,"AirDownTilt","airDownTilt", true, 1.5f,0f,0f,0f);
            //     }
            //     else if(canAirCombo && curCombo > 3 ||canAirCombo && curCombo == 0)
            //     {
            //         stopMovement(true, true);
            //         hitboxAnimation(1,"AAttack1","airCombo1", true, 1.5f,0f,1.7f,0f);
            //     }
            //     else if(canAirCombo && curCombo == 1)
            //     {
            //         stopMovement(true, true);
            //         hitboxAnimation(2,"AAttack2","airCombo2", true, 1.5f,0f,1.7f,0f);
            //     }
            //     else if(canAirCombo && curCombo == 2)
            //     {
            //         stopMovement(true, true);
            //         hitboxAnimation(2,"AAttack3","airCombo3", true, 1.5f,0f,1.7f,0f);
            //     }
            //     else if(canAirCombo && curCombo == 3 )
            //     {
            //         stopMovement(true, true);
            //         hitboxAnimation(5,"AAttack4","airCombo4", true, 1.5f,0f,1.7f,0f);
            //     }
                
            // }

            // if(attackAnimator.GetBool("UpTilt") || (holdingUp && !isTiltAttacking))
            // {
            //     stopMovement(true, true);
            //     hitboxAnimation(4,"UpTilt","UpTilt", false, 1.5f,0f,0f,0f);
            // }
            // else if(attackAnimator.GetBool("DownTilt")  || (holdingDown && !isTiltAttacking))
            // {
            //     stopMovement(true, true);
            //     hitboxAnimation(4,"DownTilt","DownTilt", false, 1.5f,0f,1f,0f);
            // }
            if(curCombo > 2 || curCombo == 0)
            {
                stopMovement(true, true);
                hitboxAnimation(1,"EAttack1","LightAttack1", false, 1.5f,0f,2f,0f);
            }
            else if(curCombo == 1)
            {
                stopMovement(true, true);
                hitboxAnimation(2,"EAttack2","LightAttack2", false, 1.5f,0f,2f,0f);
            }
            else if(curCombo == 2)
            {
                stopMovement(true, true);
                hitboxAnimation(3,"EAttack3","LightAttack3", false, 1.5f,0f,2.5f,0f);
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
            stopPlayerMovement = true;

            if(curCombo > 4 || curCombo == 0)
            {
                hitboxAnimation(1,"RAttack1","MediumAttack1",false, 1.5f,0f,2f,0f);
            }
            else if(curCombo == 1)
            {
                hitboxAnimation(2,"RAttack2","MediumAttack2",false, 1.5f,0f,2f,0f);
            }
            else if(curCombo == 2)
            {
                hitboxAnimation(2,"RAttack3","MediumAttack3",false, 1.5f,0f,2f,0f);
            } else if(curCombo == 3) 
            {
                hitboxAnimation(2,"RAttack4","MediumAttack4",false, 1.5f,0f,2f,0f);
            }
             else if(curCombo == 4) {
                hitboxAnimation(3,"RAttack5","MediumAttack5",false, 1.5f,0f,2f,0f);
            }

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

            stopPlayerMovement = true;
            
            if(curCombo > 3 || curCombo == 0)
            {
                hitboxAnimation(1,"FAttack1","HeavyAttack1", false, 1.5f,0f,2f,0f);
            }
            else if(curCombo == 1)
            {
                hitboxAnimation(2,"FAttack2","HeavyAttack2", false, 1.5f,0f,2f,0f);
            }
            else if(curCombo == 2)
            {
                hitboxAnimation(2,"FAttack3","HeavyAttack3", false, 1.5f,0f,2f,0f);
            } else if(curCombo == 3) 
            {
                hitboxAnimation(3,"FAttack4","HeavyAttack4", false, 1.5f,0f,2f,0f);
            }

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
        this.isAirAttacking = isAirAttacking;

        // Version 1 refers to when it is a first attack in the combo
        if(version == 1){
            

            isAttacking = true;

            // Animator bool example: "EAttack1"
            attackAnimator.SetBool(animatorBool,true); // Sets the first combo attack in motion

            setKnockback(knockBackX,knockBackY); // Sets knockback stats for this attack
            dashWithAttack(dashX,dashY);

            // Timer so if the player doesnt press anything it resets the combo
            if(!comboResetTimerActive){ 
                comboResetTimer = coolDownTime;
                comboResetTimerActive = true;
            }

            // animation name would be something like LightAttack1 and refers to the animation itself
            if(currentStateInfo.IsName(animationName) && isAnimating == false){
                stopPlayerMovement = false;
                stopPlayerYMovement = false;
                isAttacking = false;
                this.isAirAttacking = false;


                curCombo = 1; // For the sake of linking an attack that has a higher combo count than this one it has to set the combo back to 1
                attackAnimator.SetBool(animatorBool,false); // Sets the animation state back to Idle
                canAttack = true;
                resetKnockback(); // Resets the knockback stats for this attack
            }
        }
        
        // version 2 refers to the middle part of combos
        else if(version == 2){

            isAttacking = true;
            attackAnimator.SetBool(animatorBool,true); // Sets the first combo attack in motion

            setKnockback(knockBackX,knockBackY); // Sets knockback stats for this attack
            dashWithAttack(dashX,dashY);

            // Timer so if the player doesnt press anything it resets the combo
            if(!comboResetTimerActive){ 
                comboResetTimer = coolDownTime;
                comboResetTimerActive = true;
            }
            

            if(currentStateInfo.IsName(animationName) && isAnimating == false ){
                
                stopPlayerMovement = false;
                stopPlayerYMovement = false;
                isAttacking = false;
                this.isAirAttacking = false;

                curCombo++;
                attackAnimator.SetBool(animatorBool,false); // Sets the animation state back to Idle
                canAttack = true;
                resetKnockback(); // Resets the knockback stats for this attack
            }
        }
        // Version 3 refers to the end of the combo
        else if(version == 3){


            isAttacking = true;
            attackAnimator.SetBool(animatorBool,true); // Sets the first combo attack in motion

            setKnockback(knockBackX,knockBackY); // Sets knockback stats for this attack
            dashWithAttack(dashX,dashY);

            //Since this is the third attack things are slightly different
            comboResetTimerActive = false; // No timer needed since we are now waiting on the animation to finish
        
            
            if(currentStateInfo.IsName(animationName) && isAnimating == false ){
                this.isAirAttacking = false;
                resetAttacks(); // Reset attacks sets can attack to true
                resetKnockback(); // Resets the knockback stats for this attack
            }
        }
        // version 4 is used for the up tilt and down tilts
        else if(version == 4)
        {
            
            isAttacking = true;
            attackAnimator.SetBool(animatorBool,true); 
            

            setKnockback(knockBackX,knockBackY); // Sets knockback stats for this attack
            //dashWithAttack(dashX,dashY);

            

            if(currentStateInfo.IsName(animationName) && isAnimating == false )
            {
                
                stopPlayerMovement = false;
                stopPlayerYMovement = false;
                this.isAirAttacking = false;
                isTiltAttacking = false;
                isAttacking = false;

                attackAnimator.SetBool(animatorBool,false); 
                curCombo++;
                canAttack = true;
                resetKnockback(); // Resets the knockback stats for this attack
            }
        }
        
        // Basically number 3 but now it gets rid of the air combo so the player cant just fly
        else if(version == 5)
        {

            isAttacking = true;
            attackAnimator.SetBool(animatorBool,true); // Sets the first combo attack in motion

            setKnockback(knockBackX,knockBackY); // Sets knockback stats for this attack
            dashWithAttack(dashX,dashY);

            //Since this is the third attack things are slightly different
            comboResetTimerActive = false; // No timer needed since we are now waiting on the animation to finish
        
            
            Debug.Log(currentStateInfo.normalizedTime);

            if(currentStateInfo.IsName(animationName) && isAnimating == false ){
                canAirCombo = false;
                this.isAirAttacking = false;
                resetAttacks(); // Reset attacks sets can attack to true
                resetKnockback(); // Resets the knockback stats for this attack
            }
        }
    }


    }

    void resetKnockback(){
        setKnockback(0f,0f); // Reset knockback
    }


    // Create more In the input manager with a different letter to indicate the different buttons used
    void OnAttackE(InputValue value)
    {
        if(canInput && holdingUp){
            attackPressed = AttackType.upTiltE;
            canInput = false;
        }
        else if(canInput && holdingDown){
            attackPressed = AttackType.downTiltE;
            canInput = false;
        }
        else if(canInput){
            attackPressed = AttackType.attackE;
            canInput = false;
        }
    }

    void OnAttackR(InputValue value)
    {
        if(canInput){
            attackPressed = AttackType.attackR;
            canInput = false;
        }
    }

    void OnAttackF(InputValue value)
    {
        if(canInput){
            attackPressed = AttackType.attackF;
            canInput = false;
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
        curCombo = 0;
        currentAttackPressed = AttackType.none;
        stopPlayerMovement = false;
        stopPlayerYMovement = false;
        isAttacking = false;
        //attackBoxCollider.enabled = false;
        attackAnimator.SetBool("EAttack1",false);
        attackAnimator.SetBool("EAttack2",false);
        attackAnimator.SetBool("EAttack3",false);

        attackAnimator.SetBool("RAttack1",false);
        attackAnimator.SetBool("RAttack2",false);
        attackAnimator.SetBool("RAttack3",false);
        attackAnimator.SetBool("RAttack4",false);
        attackAnimator.SetBool("RAttack5",false);
        
        attackAnimator.SetBool("FAttack1",false);
        attackAnimator.SetBool("FAttack2",false);
        attackAnimator.SetBool("FAttack3",false);
        attackAnimator.SetBool("FAttack4",false);

        canAttack = true;
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
