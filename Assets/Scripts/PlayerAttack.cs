using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

// Enum that holds which attack is inputted by the player
enum AttackType : int{
        none,attackE, attackR, attackF, upTiltE, downTiltE, upTiltR, downTiltR, upTiltF, downTiltF
    }



public class PlayerAttack : MonoBehaviour
{
    public Animator attackAnimator;
    HitboxAnimationMethods hitboxAnimationMethods;
    
    [SerializeField] GameObject playerSprite;

    [SerializeField] GameObject attackBoxCollider; // Holds the gameobject that has the collider for the hitbox

    AttackType attackPressed; // Records the attack being pressed by the player

    AttackType currentAttackPressed; // Takes in that recorded attack when certain conditions are met

    public bool holdingUp; // Records If the player is holding up (W on the keyboard)
    
    public bool holdingDown; // Records If the player is holding down (S on the keyboard)

    public bool isOnGround; // Variable that holds if the player is currently on the ground

    public bool canAttack; // Will let us know when we can preform the next attack

    public bool canInput; // Will let us know when we record an input in currentAttackPressed from attackPressed

    public bool isAnimating; // boolean for letting us know when the hitbox is being animated

    public bool canAirCombo; // Boolean for if we are able to air combo right now (after you finish an air combo this beomes false)

    public bool isAirAttacking; // Boolean for when you are currently performing an air attack

    public bool isAttacking; // Boolean for when the player is performing an attack

    public bool isTiltAttacking; // Boolean for when the player is performing a tilt attack

    public bool comboResetTimerActive; // Boolean for when the combo reset timer is activated

    [SerializeField] float comboResetTimer; // The timer for when a combo resets

    [SerializeField] float coolDownTime = 2f; // The time the timer is set to at the beginning

    AnimatorStateInfo currentStateInfo; // Gets the current animation state

    float knockbackX; // Knockback float for the x axis

    float knockbackY; // Knockback float for the y axis

    float currentDamage; // Amount of damage the current attack will do on impact

    public float curCombo; // Keeps track of the combo number

    float curAirCombo; // Keeps track of the combo number when in the air (It has to be seperate from the ground)

    public float comboBufferTime;

    Boolean stopPlayerMovement; // For when an attack stops the player from making any more movements

    Boolean stopPlayerYMovement; // used for when we want to stop any Y movement

    public Boolean isComboBuffered; // Used to tell if the player should be allowed to enter new inputs 

    Rigidbody2D playerRigidbody; // Reference to the player's rigid body


    [SerializeField] private int playerIndex = 0;

    Boolean disablePlayerInput;

    Boolean isBlocking;

    Boolean isParrying;

    bool disableThisPlayerInput; // Boolean for when the combo reset timer is activated


    [Header("Light attack variables (3 hit combo)")]

    [SerializeField] float LA1KnockbackX;
    [SerializeField] float LA1KnockbackY;
    [SerializeField] float LA1DashX;
    [SerializeField] float LA1DashY;
    [SerializeField] float LA1Damage;
    [SerializeField] bool LA1StopMovingAll;
    [SerializeField] bool LA1StopMovingY;

    [Header("")]

    [SerializeField] float LA2KnockbackX;
    [SerializeField] float LA2KnockbackY;
    [SerializeField] float LA2DashX;
    [SerializeField] float LA2DashY;
    [SerializeField] float LA2Damage;
    [SerializeField] bool LA2StopMovingAll;
    [SerializeField] bool LA2StopMovingY;

    [Header("")]

    [SerializeField] float LA3KnockbackX;
    [SerializeField] float LA3KnockbackY;
    [SerializeField] float LA3DashX;
    [SerializeField] float LA3DashY;
    [SerializeField] float LA3Damage;
    [SerializeField] bool LA3StopMovingAll;
    [SerializeField] bool LA3StopMovingY;

    [Header("Medium attack variables (5 hit combo)")]

    [SerializeField] float MA1KnockbackX;
    [SerializeField] float MA1KnockbackY;
    [SerializeField] float MA1DashX;
    [SerializeField] float MA1DashY;
    [SerializeField] float MA1Damage;
    [SerializeField] bool MA1StopMovingAll;
    [SerializeField] bool MA1StopMovingY;

    [Header("")]
    [SerializeField] float MA2KnockbackX;
    [SerializeField] float MA2KnockbackY;
    [SerializeField] float MA2DashX;
    [SerializeField] float MA2DashY;
    [SerializeField] float MA2Damage;
    [SerializeField] bool MA2StopMovingAll;
    [SerializeField] bool MA2StopMovingY;

    [Header("")]
    [SerializeField] float MA3KnockbackX;
    [SerializeField] float MA3KnockbackY;
    [SerializeField] float MA3DashX;
    [SerializeField] float MA3DashY;
    [SerializeField] float MA3Damage;
    [SerializeField] bool MA3StopMovingAll;
    [SerializeField] bool MA3StopMovingY;

    [Header("")]
    [SerializeField] float MA4KnockbackX;
    [SerializeField] float MA4KnockbackY;
    [SerializeField] float MA4DashX;
    [SerializeField] float MA4DashY;
    [SerializeField] float MA4Damage;
    [SerializeField] bool MA4StopMovingAll;
    [SerializeField] bool MA4StopMovingY;

    [Header("")]
    [SerializeField] float MA5KnockbackX;
    [SerializeField] float MA5KnockbackY;
    [SerializeField] float MA5DashX;
    [SerializeField] float MA5DashY;
    [SerializeField] float MA5Damage;
    [SerializeField] bool MA5StopMovingAll;
    [SerializeField] bool MA5StopMovingY;


    [Header("Heavy attack variables (4 hit combo)")]
    
    [SerializeField] float HA1KnockbackX;
    [SerializeField] float HA1KnockbackY;
    [SerializeField] float HA1DashX;
    [SerializeField] float HA1DashY;
    [SerializeField] float HA1Damage;
    [SerializeField] bool HA1StopMovingAll;
    [SerializeField] bool HA1StopMovingY;

    [Header("")]
    [SerializeField] float HA2KnockbackX;
    [SerializeField] float HA2KnockbackY;
    [SerializeField] float HA2DashX;
    [SerializeField] float HA2DashY;
    [SerializeField] float HA2Damage;
    [SerializeField] bool HA2StopMovingAll;
    [SerializeField] bool HA2StopMovingY;

    [Header("")]
    [SerializeField] float HA3KnockbackX;
    [SerializeField] float HA3KnockbackY;
    [SerializeField] float HA3DashX;
    [SerializeField] float HA3DashY;
    [SerializeField] float HA3Damage;
    [SerializeField] bool HA3StopMovingAll;
    [SerializeField] bool HA3StopMovingY;

    [Header("")]
    [SerializeField] float HA4KnockbackX;
    [SerializeField] float HA4KnockbackY;
    [SerializeField] float HA4DashX;
    [SerializeField] float HA4DashY;
    [SerializeField] float HA4Damage;
    [SerializeField] bool HA4StopMovingAll;
    [SerializeField] bool HA4StopMovingY;


    [Header("Up Tilt (On the ground)")]
    [SerializeField] float UTKnockbackX;
    [SerializeField] float UTKnockbackY;
    [SerializeField] float UTDashX;
    [SerializeField] float UTDashY;
    [SerializeField] float UTDamage;
    [SerializeField] bool UTStopMovingAll;
    [SerializeField] bool UTStopMovingY;

    [Header("In air Up Tilt ")]
    [SerializeField] float AUTKnockbackX;
    [SerializeField] float AUTKnockbackY;
    [SerializeField] float AUTDashX;
    [SerializeField] float AUTDashY;
    [SerializeField] float AUTDamage;
    [SerializeField] bool AUTStopMovingAll;
    [SerializeField] bool AUTStopMovingY;

    [Header("Down Tilt (On the ground)")]
    [SerializeField] float DTKnockbackX;
    [SerializeField] float DTKnockbackY;
    [SerializeField] float DTDashX;
    [SerializeField] float DTDashY;
    [SerializeField] float DTDamage;
    [SerializeField] bool DTStopMovingAll;
    [SerializeField] bool DTStopMovingY;

    [Header("In air Down Tilt ")]
    [SerializeField] float ADTKnockbackX;
    [SerializeField] float ADTKnockbackY;
    [SerializeField] float ADTDashX;
    [SerializeField] float ADTDashY;
    [SerializeField] float ADTDamage;
    [SerializeField] bool ADTStopMovingAll;
    [SerializeField] bool ADTStopMovingY;

    [Header("Air Combo (4 hit combo)")]
    [SerializeField] float A1KnockbackX;
    [SerializeField] float A1KnockbackY;
    [SerializeField] float A1DashX;
    [SerializeField] float A1DashY;
    [SerializeField] float A1Damage;
    [SerializeField] bool A1StopMovingAll;
    [SerializeField] bool A1StopMovingY;

    [Header("")]
    [SerializeField] float A2KnockbackX;
    [SerializeField] float A2KnockbackY;
    [SerializeField] float A2DashX;
    [SerializeField] float A2DashY;
    [SerializeField] float A2Damage;
    [SerializeField] bool A2StopMovingAll;
    [SerializeField] bool A2StopMovingY;

    [Header("")]
    [SerializeField] float A3KnockbackX;
    [SerializeField] float A3KnockbackY;
    [SerializeField] float A3DashX;
    [SerializeField] float A3DashY;
    [SerializeField] float A3Damage;
    [SerializeField] bool A3StopMovingAll;
    [SerializeField] bool A3StopMovingY;

    [Header("")]
    [SerializeField] float A4KnockbackX;
    [SerializeField] float A4KnockbackY;
    [SerializeField] float A4DashX;
    [SerializeField] float A4DashY;
    [SerializeField] float A4Damage;
    [SerializeField] bool A4StopMovingAll;
    [SerializeField] bool A4StopMovingY;

    bool checkCanAttackBroke;


    // Start is called before the first frame update
    void Start()
    {
        attackPressed = AttackType.none; 
        holdingUp = false;
        holdingDown = false;
        isOnGround = false;
        

        // Would need to change the code surrounding this if I want to use multiple hitboxes
        attackAnimator = playerSprite.GetComponent<Animator>(); // Reference to the attack hitbox animator


        canAttack = true; 
        canAirCombo = true;
        canInput = true;
        isAttacking = false;
        isTiltAttacking = false;
        comboResetTimerActive = false;
        comboResetTimer = 0;


        knockbackX = 0; 
        knockbackY = 0;

        curCombo = 0; 
        curAirCombo = 0;

        stopPlayerMovement = false;
        stopPlayerYMovement = false;

        isComboBuffered = false;

        disablePlayerInput = false;
        
        playerRigidbody = transform.GetComponent<Rigidbody2D>();

        hitboxAnimationMethods = playerSprite.GetComponent<HitboxAnimationMethods>();

        checkCanAttackBroke = false;
    }


	// Update is called once per frame
	 void Update()
    {
        attackColliderUpdate(); // Update function for the attack collider
        gameManagerUpdate(); // Used to check if the player is on the ground
        updateAirCombo(); // Used for reseting the aircombo when the player hits the ground
        attacks(); // Update function for the attack system
        //resetTimerCheck(); // Update function for resetting the combo timer
        updateMovement(); // If the player is to have their movement stopped it is sent to here
        
    }
    
    private void FixedUpdate() {
        resetTimerCheck();
    }
    public int GetPlayerIndex(){
        return playerIndex;
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

    // Checks if the player is on the ground and stores it in that variable within the game manager
    void gameManagerUpdate(){
        disablePlayerInput = GameManager.Instance.disablePlayerInputs;
        if(transform.tag == "Player1"){
            isOnGround = GameManager.Instance.player1IsOnGround;
            isBlocking = GameManager.Instance.p1IsBlocking;
            isParrying = GameManager.Instance.p1IsParrying;
            GameManager.Instance.isP1Attacking = isAttacking;

            disableThisPlayerInput = GameManager.Instance.p1InputDisabled;
        }
        else{
            isOnGround = GameManager.Instance.player2IsOnGround;
            isBlocking = GameManager.Instance.p2IsBlocking;
            isParrying = GameManager.Instance.p2IsParrying;
            GameManager.Instance.isP2Attacking = isAttacking;

            disableThisPlayerInput = GameManager.Instance.p2InputDisabled;
        }
    }

    // Used for reseting the aircombo once the player hits the ground
    void updateAirCombo(){
        if(isOnGround && curAirCombo > 0){
            curAirCombo = 0;
            canAirCombo = true;
            resetAirAttacks();
            canAttack = true;
        }

        if(isOnGround && canAirCombo == false){
            canAirCombo = true;
            resetAirAttacks();
            canAttack = true;
        }
    } 

    
    // Stops movement depending on the booleans entered
    void stopMovement(Boolean stopPlayerMovement, Boolean stopPlayerYMovement){
        this.stopPlayerMovement = stopPlayerMovement; 
        this.stopPlayerYMovement = stopPlayerYMovement;
    }  

    
    // Cleans up the animation booleans in case anything is left as true by the end of an update cycle.
    // Might be better to remove this at some point but it fixes a few problems so im leaving it for now
    void booleanCleanup(){
        if(isAnimating == false){
            resetAnimationBooleans();
        }
    }

    // When hitting the ground while in an air combo this function is called to reset the air attacks
    void airToGroundReset(){
        curCombo = 0;
        curAirCombo = 0;
        isAttacking = false;
        resetAirAttacks();
        canAttack = true;
    }
    

    // This is the update function for the attack hitbox code
    public void attacks()
    {
        
        booleanCleanup(); // Used for cleaning up any boolean values that were not disabled in the last update cycle

        currentStateInfo = attackAnimator.GetCurrentAnimatorStateInfo(0); // Gets the current state of the animator

        updateInput(); // Updates the caninput variable in this function with the game manager variable

        // When you are able to attack it records the key you press
        if(canAttack == true && !isComboBuffered)
        {
            // You arent able to record your next attack input if the current attack pressed is any key
            if(currentAttackPressed != AttackType.none)
            {
                canAttack = false; // Sets canAttack to false to prevent a backlog of attacks from building up
                //Debug.Log("Current Attack Pressed:" + currentAttackPressed);
            }
            else{
                // records the players attack to be used next
                currentAttackPressed = attackPressed;
                attackPressed = AttackType.none; // Resets the attack pressed for the next loop
            }
        }

        // When the player performs an up tilt ( W + Any attack button)
        if(currentAttackPressed == AttackType.upTiltE || currentAttackPressed == AttackType.upTiltR || currentAttackPressed == AttackType.upTiltF )
        {
            // IMPORTANT: Each tilt should do different damage and have different speeds of animation

            // If you are on the ground or while in the air up tilt animation 
            if(isAirAttacking == true || !isOnGround )
            {
                // If you hit the ground it resets the animations
                if(isOnGround)
                {
                    airToGroundReset();
                }
                else
                {
                    // In the air
                    //stopMovement(AUTStopMovingAll, AUTStopMovingY); // Dont stop movement for this
                    hitboxAnimation(4,"AirUpTilt","airUpTilt", true, AUTKnockbackX,AUTKnockbackY,AUTDashX,AUTDashY,AUTDamage);
                    
                }
                
            }
            else
            {
                // On the ground
                //stopMovement(UTStopMovingAll, UTStopMovingY); // Stops movement for this
                hitboxAnimation(4,"UpTilt","UpTilt", false, UTKnockbackX,UTKnockbackY,UTDashX,UTDashY,UTDamage);
            }

            updateAttacks(); // Knockback update
        }

        // When the player performs an down tilt ( S + Any attack button)
        if(currentAttackPressed == AttackType.downTiltE || currentAttackPressed == AttackType.downTiltR || currentAttackPressed == AttackType.downTiltF)
        {
            // IMPORTANT: Each tilt should do different damage and have different speeds of animation

            // If you are on the ground or while in the air down tilt animation 
            if(isAirAttacking == true || !isOnGround )
            {
                // If you hit the ground it resets the attack 
                if(isOnGround)
                {
                    airToGroundReset();
                }
                else
                {
                    // In the air
                    stopMovement(ADTStopMovingAll, ADTStopMovingY); // Dont freeze movement
                    hitboxAnimation(4,"AirDownTilt","airDownTilt", true, ADTKnockbackX,ADTKnockbackY,ADTDashX,ADTDashY,ADTDamage);
                }
                
            }
            else
            {
                // On the ground
                stopMovement(DTStopMovingAll, DTStopMovingY); // Dont freeze movement
                hitboxAnimation(4,"DownTilt","DownTilt", false, DTKnockbackX,DTKnockbackY,UTDashX,DTDashY,DTDamage);
            }

            updateAttacks(); // Updates knockback
        }

        // When the user enters an E attack (presses E) Will change the bind most likely
        if(currentAttackPressed == AttackType.attackE)
        {
            // When the player is in the air or in an air attack animation
            if(isAirAttacking == true || !isOnGround ){
                // If they hit the ground while in an air attack animation it resets all air animations
                if(isOnGround)
                {
                    airToGroundReset();
                }

                // 4 hit air combo attack

                else if(canAirCombo && curAirCombo > 3 ||canAirCombo && curAirCombo == 0)
                {
                    stopMovement(A1StopMovingAll, A1StopMovingY); 
                    hitboxAnimation(1,"AAttack1","airCombo1", true, A1KnockbackX,A1KnockbackY,A1DashX,A1DashY,A1Damage);
                }
                else if(canAirCombo && curAirCombo == 1)
                {
                    stopMovement(A2StopMovingAll, A2StopMovingY);
                    hitboxAnimation(2,"AAttack2","airCombo2", true, A2KnockbackX,A2KnockbackY,A2DashX,A2DashY,A2Damage);
                }
                else if(canAirCombo && curAirCombo == 2)
                {
                    stopMovement(A3StopMovingAll, A3StopMovingY);
                    hitboxAnimation(2,"AAttack3","airCombo3", true, A3KnockbackX,A3KnockbackY,A3DashX,A3DashY,A3Damage);
                }
                else if(canAirCombo && curAirCombo == 3 )
                {
                    stopMovement(A4StopMovingAll, A4StopMovingY);
                    hitboxAnimation(5,"AAttack4","airCombo4", true, A4KnockbackX,A4KnockbackY,A4DashX,A4DashY,A4Damage);
                }
                
            }

            // 3 hit light attack combo

            else if(curCombo > 2 || curCombo == 0)
            {
                stopMovement(LA1StopMovingAll, LA1StopMovingY);
                hitboxAnimation(1,"EAttack1","LightAttack1", false, LA1KnockbackX,LA1KnockbackY,LA1DashX,LA1DashY,LA1Damage);
            }
            else if(curCombo == 1)
            {
                stopMovement(LA2StopMovingAll, LA2StopMovingY);
                hitboxAnimation(2,"EAttack2","LightAttack2", false, LA2KnockbackX,LA2KnockbackY,LA2DashX,LA2DashY,LA2Damage);
            }
            else if(curCombo == 2)
            {
                stopMovement(LA3StopMovingAll, LA3StopMovingY);
                hitboxAnimation(3,"EAttack3","LightAttack3", false, LA3KnockbackX,LA3KnockbackY,LA3DashX,LA3DashY,LA3Damage);
            }


            // Updates the knockback variables in the game instance
            updateAttacks();

        }

        // When the player enters an R attack (Presses R) Will change the bind most likely
        if (currentAttackPressed == AttackType.attackR)
        {
            // Exact same thing as the E combo but it should deal more damage and be slower or something
            if(isAirAttacking == true || !isOnGround ){
                // If they hit the ground while in an air attack animation it resets all air animations
                if(isOnGround)
                {
                    airToGroundReset();
                }

                // 4 hit air combo attack

                else if(canAirCombo && curAirCombo > 3 ||canAirCombo && curAirCombo == 0)
                {
                    stopMovement(A1StopMovingAll, A1StopMovingY); 
                    hitboxAnimation(1,"AAttack1","airCombo1", true, A1KnockbackX,A1KnockbackY,A1DashX,A1DashY,A1Damage);
                }
                else if(canAirCombo && curAirCombo == 1)
                {
                    stopMovement(A2StopMovingAll, A2StopMovingY);
                    hitboxAnimation(2,"AAttack2","airCombo2", true, A2KnockbackX,A2KnockbackY,A2DashX,A2DashY,A2Damage);
                }
                else if(canAirCombo && curAirCombo == 2)
                {
                    stopMovement(A3StopMovingAll, A3StopMovingY);
                    hitboxAnimation(2,"AAttack3","airCombo3", true, A3KnockbackX,A3KnockbackY,A3DashX,A3DashY,A3Damage);
                }
                else if(canAirCombo && curAirCombo == 3 )
                {
                    stopMovement(A4StopMovingAll, A4StopMovingY);
                    hitboxAnimation(5,"AAttack4","airCombo4", true, A4KnockbackX,A4KnockbackY,A4DashX,A4DashY,A4Damage);
                }
                
            }
            
            // 5 hit medium attack combo

            else if(curCombo > 4 || curCombo == 0)
            {
                stopMovement(MA1StopMovingAll,MA1StopMovingY);
                hitboxAnimation(1,"RAttack1","MediumAttack1",false, MA1KnockbackX,MA1KnockbackY,MA1DashX,MA1DashY,MA1Damage);
            }
            else if(curCombo == 1)
            {
                stopMovement(MA2StopMovingAll,MA2StopMovingY);
                hitboxAnimation(2,"RAttack2","MediumAttack2",false, MA2KnockbackX,MA2KnockbackY,MA2DashX,MA2DashY,MA2Damage);
            }
            else if(curCombo == 2)
            {
                stopMovement(MA3StopMovingAll,MA3StopMovingY);
                hitboxAnimation(2,"RAttack3","MediumAttack3",false, MA3KnockbackX,MA3KnockbackY,MA3DashX,MA3DashY,MA3Damage);
            } 
            else if(curCombo == 3) 
            {
                stopMovement(MA4StopMovingAll,MA4StopMovingY);
                hitboxAnimation(2,"RAttack4","MediumAttack4",false, MA4KnockbackX,MA4KnockbackY,MA4DashX,MA4DashY,MA4Damage);
            }
            else if(curCombo == 4) {
                stopMovement(MA5StopMovingAll,MA5StopMovingY);
                hitboxAnimation(3,"RAttack5","MediumAttack5",false, MA5KnockbackX,MA5KnockbackY,MA5DashX,MA5DashY,MA5Damage);
            }

            updateAttacks(); // Updates knockback
        }

        if (currentAttackPressed == AttackType.attackF)
        {

            // Exact same thing as the E combo but it should deal more damage and be slower or something
            if(isAirAttacking == true || !isOnGround ){
                // If they hit the ground while in an air attack animation it resets all air animations
                if(isOnGround)
                {
                    airToGroundReset();
                }

                // 4 hit air combo attack

                else if(canAirCombo && curAirCombo > 3 ||canAirCombo && curAirCombo == 0)
                {
                    stopMovement(A1StopMovingAll, A1StopMovingY); 
                    hitboxAnimation(1,"AAttack1","airCombo1", true, A1KnockbackX,A1KnockbackY,A1DashX,A1DashY,A1Damage);
                }
                else if(canAirCombo && curAirCombo == 1)
                {
                    stopMovement(A2StopMovingAll, A2StopMovingY);
                    hitboxAnimation(2,"AAttack2","airCombo2", true, A2KnockbackX,A2KnockbackY,A2DashX,A2DashY,A2Damage);
                }
                else if(canAirCombo && curAirCombo == 2)
                {
                    stopMovement(A3StopMovingAll, A3StopMovingY);
                    hitboxAnimation(2,"AAttack3","airCombo3", true, A3KnockbackX,A3KnockbackY,A3DashX,A3DashY,A3Damage);
                }
                else if(canAirCombo && curAirCombo == 3 )
                {
                    stopMovement(A4StopMovingAll, A4StopMovingY);
                    hitboxAnimation(5,"AAttack4","airCombo4", true, A4KnockbackX,A4KnockbackY,A4DashX,A4DashY,A4Damage);
                }
                
            }
            
            else if(curCombo > 3 || curCombo == 0)
            {
                stopMovement(HA1StopMovingAll, HA1StopMovingY);
                hitboxAnimation(1,"FAttack1","HeavyAttack1", false, HA1KnockbackX,HA1KnockbackY,HA1DashX,HA1DashY,HA1Damage);
            }
            else if(curCombo == 1)
            {
                stopMovement(HA2StopMovingAll, HA2StopMovingY);
                hitboxAnimation(2,"FAttack2","HeavyAttack2", false, HA2KnockbackX,HA2KnockbackY,HA2DashX,HA2DashY,HA2Damage);
            }
            else if(curCombo == 2)
            {
                stopMovement(HA3StopMovingAll,HA3StopMovingY);
                hitboxAnimation(2,"FAttack3","HeavyAttack3", false, HA3KnockbackX,HA3KnockbackY,HA3DashX,HA3DashY,HA3Damage);
            } else if(curCombo == 3) 
            {
                stopMovement(HA4StopMovingAll,HA4StopMovingY);
                hitboxAnimation(3,"FAttack4","HeavyAttack4", false, HA4KnockbackX,HA4KnockbackY,HA4DashX,HA4DashY,HA4Damage);
            }

            updateAttacks();
    }

    // Gets the input from the Gamemanager (These variables are decided by events in the animations themselves)
    void updateInput()
    {
        if(transform.tag == "Player1"){
            canInput = GameManager.Instance.canInputP1;
            isAnimating = GameManager.Instance.isHitBoxAnimatingP1;
        }
        else{
            canInput = GameManager.Instance.canInputP2;
            isAnimating = GameManager.Instance.isHitBoxAnimatingP2;
        }
    }

    // The end of the hitbox animations have certain differences that are called here
    

    // IMPORTANT: Need to add a variable for damage dealt, this will change depending on the attack
    void hitboxAnimation(int version, String animatorBool, String animationName, Boolean isAirAttacking, float knockBackX, float knockBackY, float dashX, float dashY, float damage){
        
        this.isAirAttacking = isAirAttacking;

        isAttacking = true;
        attackAnimator.SetBool("isAttacking",true); // Sets the attacking bool in the animator to true

        // Animator bool example: "EAttack1"
        attackAnimator.SetBool(animatorBool,true); // Sets the corrisponding boolean to true

        //Damage calculation
        currentDamage = damage;
        if((gameObject.CompareTag("Player1") && GameManager.Instance.isOffensiveP1) || (gameObject.CompareTag("Player2") && GameManager.Instance.isOffensiveP2))
        {
            //If the player is offensive, increase damage by 50%
            currentDamage *= 1.5f;
        }
        if((gameObject.CompareTag("Player1") && GameManager.Instance.isOffensiveP2) || (gameObject.CompareTag("Player2") && GameManager.Instance.isOffensiveP1))
        {
            //If the other player is offensive, increase damage by 20%
            currentDamage *= 1.2f;
        }

        setKnockback(knockBackX,knockBackY); // Sets knockback stats for this attack

        
        
        // As long as one dash variable isnt 0 the dash function is called
        if(dashX != 0 || dashY != 0)
        {
            dashWithAttack(dashX,dashY); // dashes the player on the x and y axis
        }

        // Timer so if the player doesnt press anything it resets the combo
        if(version != 3 && version != 5)
        {
            comboResetTimer = coolDownTime;
            comboResetTimerActive = true;
        }
        //Debug.Log("Current State Info: " + currentStateInfo.IsName(animationName));
        //Debug.Log("Is Animating: " + isAnimating);
        
        // animation name would be something like LightAttack1 and refers to the animation itself
        if(currentStateInfo.IsName(animationName) && !isAnimating){
            completeAnimation(version, animatorBool, animationName, isAirAttacking);
        }
    }
}
    
    public void completeAnimation(int version, String animatorBool, String animationName, Boolean isAirAttacking){

        // Refers to the first attack in an attack combo
        if(version == 1)
        {
            // Re-enables movement for the player
            stopPlayerMovement = false;
            stopPlayerYMovement = false;
            
            // The player is no longer attacking (Covers both ground and air just in case)
            isAttacking = false;
            attackAnimator.SetBool("isAttacking",false); // Sets the attacking bool in the animator to false
            this.isAirAttacking = false;

            // Air attacking variable below is different than the one above
            if(isAirAttacking)
            {
                // Set the air combo to 1 to account for the player going above the max combo count
                curAirCombo = 1; 
            }
            else
            {
                // Set the combo to 1 to account for the player going above the max combo count
                curCombo = 1; 
            }

            //Debug.Log("Current Combo Complete: " + animationName);

            attackAnimator.SetBool(animatorBool,false); // Sets the animation state back to Idle
            canAttack = true; // The player can not do their next attack
            currentAttackPressed = AttackType.none; // The current attack being done is set to none
            resetKnockback(); // Resets the knockback stats 
        }

        // Refers to the middle attacks of a combo
        else if(version == 2)
        {
            // Re-enables player movement
            stopPlayerMovement = false;
            stopPlayerYMovement = false;

            // The player is no longer attacking
            isAttacking = false;
            attackAnimator.SetBool("isAttacking",false); // Sets the attacking bool in the animator to false
            this.isAirAttacking = false;

            // When air attacking increase air combo count and when ground attacking increase regular combo count
            if(isAirAttacking)
            {
                curAirCombo++; 
            }
            else
            {
                curCombo++;
            }
            
            //Debug.Log("Current Combo Complete: " + animationName);
            
            attackAnimator.SetBool(animatorBool,false); // Sets the animation state back to Idle
            canAttack = true; // The player can not do their next attack
            currentAttackPressed = AttackType.none; // The current attack being done is set to none
            resetKnockback(); // Resets the knockback stats 
        }

        // Refers to the last attack in a combo
        else if(version == 3)
        {
            //Debug.Log("Current Combo Complete: " + animationName);

            this.isAirAttacking = false;
            resetAttacks(); // Reset attacks sets can attack to true
            currentAttackPressed = AttackType.none; // The current attack being done is set to none
            resetKnockback(); // Resets the knockback stats for this attack
            isComboBuffered = true; // Sets the buffer to true
            StartCoroutine(comboBuffer());
        }

        // Refers to any tilt attacks
        else if(version == 4)
        {
            // Re-enables player movement
            stopPlayerMovement = false;
            stopPlayerYMovement = false;

            // The player is no longer attacking
            this.isAirAttacking = false;
            isTiltAttacking = false;
            isAttacking = false;
            attackAnimator.SetBool("isAttacking",false); // Sets the attacking bool in the animator to false

            attackAnimator.SetBool(animatorBool,false); // Sets the animation state back to Idle
            curCombo++; // Increases the combo
            canAttack = true; // The player can not do their next attack
            currentAttackPressed = AttackType.none; // The current attack being done is set to none
            resetKnockback(); // Resets the knockback stats 
        }
        // Refers to the final attack in an air combo
        else if(version == 5)
        {
            canAirCombo = false;
            this.isAirAttacking = false;
            resetAttacks(); // Reset attacks sets can attack to true
            resetAirAttacks();
            currentAttackPressed = AttackType.none;
            resetKnockback(); // Resets the knockback stats for this attack
            isComboBuffered = true; // Sets the buffer to true
            StartCoroutine(comboBuffer());
        }
    }

    // Small healper function for resetting knockback
    void resetKnockback()
    {
        setKnockback(0f,0f); // Reset knockback
    }

    
    // Updates the stop movement variables in the game manager
    void updateMovement()
    {
        if(transform.tag == "Player1")
        {
            
            GameManager.Instance.UpdateStopMovement(1, stopPlayerMovement, 0);
            GameManager.Instance.UpdateStopMovementY(1, stopPlayerYMovement, 0);
        }
        else{
            GameManager.Instance.UpdateStopMovement(2, stopPlayerMovement, 0);
            GameManager.Instance.UpdateStopMovementY(2, stopPlayerYMovement, 0);
        }
    }

    // updates the knockback variables in the game manager
    void updateAttacks()
    {
        if(gameObject.CompareTag("Player1"))
        {
            GameManager.Instance.P1AttackKnockBackX = knockbackX;
            GameManager.Instance.P1AttackKnockBackY = knockbackY;
            GameManager.Instance.p1AttackDamage = currentDamage;
        }
        else if (gameObject.CompareTag("Player2")) 
        {
            GameManager.Instance.P2AttackKnockBackX = knockbackX;
            GameManager.Instance.P2AttackKnockBackY = knockbackY;
            GameManager.Instance.p2AttackDamage = currentDamage;
        }
        else
        {
            // Debug.Log("Error assigning knockback to players");
        }
    }

    // This code is meant to act as a combo reset, when the player is on the 1st attack and doesnt press anything within a certain time then the combo resets to 0 and all animation booleans are set to false
void resetTimerCheck()
{ 
    if (comboResetTimerActive)
    {
        if(comboResetTimer > 0.01f)
            {
                comboResetTimer -= Time.fixedDeltaTime;
//                attackAnimator.SetFloat("ComboResetTimer", comboResetTimer);
            }
            else
            {
                comboResetTimer = 0;
                resetAttacks();
                comboResetTimerActive = false;
                //StartCoroutine(comboBuffer());
            }
    }
}
    // resets every animation boolean in the animator
    void resetAnimationBooleans()
    {
        attackAnimator.SetBool("AAttack1",false);
        attackAnimator.SetBool("AAttack2",false);
        attackAnimator.SetBool("AAttack3",false);
        attackAnimator.SetBool("AAttack4",false);
        attackAnimator.SetBool("AirDownTilt",false);
        attackAnimator.SetBool("AirUpTilt",false);

        attackAnimator.SetBool("UpTilt",false);
        attackAnimator.SetBool("DownTilt",false);

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
    }

    // Resets the air attacks (Usually used when you hit the ground or use the final attack in an air combo)
    void resetAirAttacks()
    {
        curCombo = 0;
        curAirCombo = 0;
        currentAttackPressed = AttackType.none;
        stopPlayerMovement = false;
        stopPlayerYMovement = false;
        isAttacking = false;
        attackAnimator.SetBool("isAttacking", false);
        //attackBoxCollider.enabled = false;
        attackAnimator.SetBool("AAttack1",false);
        attackAnimator.SetBool("AAttack2",false);
        attackAnimator.SetBool("AAttack3",false);
        attackAnimator.SetBool("AAttack4",false);
        attackAnimator.SetBool("AirDownTilt",false);
        attackAnimator.SetBool("AirUpTilt",false);

        resetAttacks();
        isAirAttacking = false;
    }

    // Resets attack state to idle
    void resetAttacks()
    {
        curCombo = 0;
        stopPlayerMovement = false;
        stopPlayerYMovement = false;
        isAttacking = false;
        attackAnimator.SetBool("isAttacking", false);
        currentAttackPressed = AttackType.none;
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
    
    }


    // Small helper function for setting the knockback variables
    void setKnockback(float x , float y)
    {
        knockbackX = x;
        knockbackY = y;
    }

    // Small helper function for dashing
    void dashWithAttack(float x, float y)
    {
        if(!isBlocking && !isParrying)
        {
            if(stopPlayerMovement)
            {
                playerRigidbody.velocity = new Vector2(0,playerRigidbody.velocity.y);
            }
            playerRigidbody.AddForce(new Vector2(x * transform.localScale.x ,y), ForceMode2D.Impulse);
        }
    }


    // Create more In the input manager with a different letter to indicate the different buttons used
    public void AttackE(CallbackContext context)
    {
        //if(isComboBuffered) return;
        if(context.started && !disablePlayerInput  && !disableThisPlayerInput && !isBlocking && !isParrying)
        {
            // When holding up you perform an uptilt
            if(canInput && holdingUp){
                attackPressed = AttackType.upTiltE;
                canInput = false; // After input is recorded set this to false
            }
            // When holding down you perform an downtilt
            else if(canInput && holdingDown){
                attackPressed = AttackType.downTiltE;
                canInput = false; // After input is recorded set this to false
            }
            // Regular attack
            else if(canInput){
                attackPressed = AttackType.attackE;
                canInput = false; // After input is recorded set this to false
            }



             //sets aggro to false, change in future
            if(gameObject.CompareTag("Player1"))
            {
                GameManager.Instance.p1Aggro = false;
            }
            else if(gameObject.CompareTag("Player2"))
            {
                GameManager.Instance.P2Aggro = false;
            }
            AudioManager.Instance.StateChange();
        }
        
        
        
    }

    public void AttackR(CallbackContext context)
    {
        //if(isComboBuffered) return;
        if(context.started && !disablePlayerInput  && !disableThisPlayerInput && !isBlocking && !isParrying)
        {
            // When holding up you perform an uptilt
            if(canInput && holdingUp){
                attackPressed = AttackType.upTiltR;
                canInput = false; // After input is recorded set this to false
            }
            // When holding down you perform an downtilt
            else if(canInput && holdingDown){
                attackPressed = AttackType.downTiltR;
                canInput = false; // After input is recorded set this to false
            }
            // Regular attack
            else if(canInput){
                attackPressed = AttackType.attackR;
                canInput = false; // After input is recorded set this to false
                
                if(hitboxAnimationMethods != null && attackAnimator.GetBool("RAttack1") == true)
                {
                    hitboxAnimationMethods.QueueNextAttack();
                    // Debug.Log("Queue Attack is called");
                }
        } 
            
            //sets aggro to false, change in future
            if(gameObject.CompareTag("Player1"))
            {
                GameManager.Instance.p1Aggro = true;
            }
            else if(gameObject.CompareTag("Player2"))
            {
                GameManager.Instance.P2Aggro = true;
            }
            AudioManager.Instance.StateChange();
        }
    }

    public void AttackF(CallbackContext context)
    {
        //if(isComboBuffered) return;
        if(context.started && !disablePlayerInput && !disableThisPlayerInput && !isBlocking && !isParrying)
        {
            // When holding up you perform an uptilt
            if(canInput && holdingUp){
                attackPressed = AttackType.upTiltF;
                canInput = false; // After input is recorded set this to false
            }
            // When holding down you perform an downtilt
            else if(canInput && holdingDown){
                attackPressed = AttackType.downTiltF;
                canInput = false; // After input is recorded set this to false
            }
            // Regular attack
            else if(canInput){
                attackPressed = AttackType.attackF;
                canInput = false; // After input is recorded set this to false
            }

            //sets aggro to false, change in future
            if(gameObject.CompareTag("Player1"))
            {
                GameManager.Instance.p1Aggro = true;
            }
            else if(gameObject.CompareTag("Player2"))
            {
                GameManager.Instance.P2Aggro = true;
            }
            AudioManager.Instance.StateChange();
        }
    }

    public void UpPressed(CallbackContext context){

        // If the input isnt 0 then the player is holding Up
        if (context.started && !disablePlayerInput && !disableThisPlayerInput)
        {
            // Debug.Log("UP");
            holdingUp = true;
        }
        else if (context.canceled)
        {
            // Debug.Log("NO UP");
            holdingUp = false;
        }
    }

    public void DownPressed(CallbackContext context){
        // If the input isnt 0 then the player is holding Down
        if (context.started && !disablePlayerInput  && !disableThisPlayerInput)
        {
            //Debug.Log("DOWN");
            holdingDown = true;
        }
        else if (context.canceled)
        {
            //Debug.Log("NO DOWN");
            holdingDown = false;
        }
    }


    private IEnumerator comboBuffer(){
        yield return new WaitForSeconds(comboBufferTime);

        canAttack = true;
        isComboBuffered = false;
        
    }
}
