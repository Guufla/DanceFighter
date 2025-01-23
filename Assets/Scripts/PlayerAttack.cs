using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

enum attackType : int{
        none,attackE, attackR, attackF
    }

public class PlayerAttack : MonoBehaviour
{
    Animator attackAnimator;

    GameObject attackBoxObject;

    CapsuleCollider2D attackBoxCollider;

    attackType attackPressed;

    attackType currentAttackPressed;

    bool canAttack;

    bool comboResetTimerActive;

    [SerializeField] float comboResetTimer;

    [SerializeField] float coolDownTime = 2f;

    AnimatorStateInfo currentStateInfo;


    float knockbackX;

    float knockbackY;

    float curCombo;








    // Start is called before the first frame update
    void Start()
    {
        attackPressed = attackType.none; // Used to signify which attack button is pressed

        // Would need to change the code surrounding this if I want to use multiple hitboxes
        attackBoxObject = transform.GetChild(2).gameObject; // Gets the attack hitbox game object



        attackAnimator = attackBoxObject.GetComponent<Animator>(); // Reference to the attack hitbox animator
        attackBoxCollider = attackBoxObject.GetComponent<CapsuleCollider2D>(); // Reference to the actual collider of the attack box object


        canAttack = true; // Determines if the player can attack or not
        comboResetTimerActive = false; // used for the reset combo timer (Explained more near that section)
        comboResetTimer = 0;


        knockbackX = 0; 
        knockbackY = 0;

        curCombo = 0; // Keeps track of the current combo. Used to move from one attack to the next like light attack 1 to light attack 2


    }

    // Update is called once per frame
    void Update()
    {
        attackColliderUpdate(); // Update function for the attack collider
        newAttacks(); // Update function for the attack system
        resetTimerCheck(); // Update function for resetting the combo timer
    }

    
    void attackColliderUpdate(){

        // Determines if the attack hitbox object should be enabled or disabled. In the actual game there wont be a sprite so its like enabling and disabling the hitbox itself
        if(currentAttackPressed == attackType.none){
            attackBoxObject.SetActive(false);
        }
        else{
            attackBoxObject.SetActive(true);
        }
    }

    // This is the new attack system I setup, old one is commented out for now but this one sets up the next attack based on the button pressed during the combo animation.
    void newAttacks()
    {
        currentStateInfo = attackAnimator.GetCurrentAnimatorStateInfo(0); // Gets the current state of the animator

        
        // When you are able to attack it records the key you press
        if(canAttack == true){
            currentAttackPressed = attackPressed;
            attackPressed = attackType.none; // Resets the attack pressed for the next loop
        }

        if(currentAttackPressed == attackType.attackE){
            
            canAttack = false; // Sets canAttack to false to prevent a backlog of attacks from building up

            if(curCombo == 0){
                attackAnimator.SetBool("EAttack1",true); // Sets the first combo attack in motion

                setKnockback(1.5f,0f); // Sets knockback stats for this attack

                // Timer so if the player doesnt press anything it resets the combo
                if(!comboResetTimerActive){ 
                    comboResetTimer = coolDownTime;
                    comboResetTimerActive = true;
                }

                if(currentStateInfo.IsName("LightAttack1") && currentStateInfo.normalizedTime >= 1 ){
                    canAttack = true;
                    curCombo++;
                    attackAnimator.SetBool("EAttack1",false); // Sets the animation state back to Idle
                    resetKnockback(); // Resets the knockback stats for this attack
                }
            }
            else if(curCombo == 1){
                attackAnimator.SetBool("EAttack2",true); // Sets the first combo attack in motion

                setKnockback(1.5f,0f); // Sets knockback stats for this attack

                // Timer so if the player doesnt press anything it resets the combo
                if(!comboResetTimerActive){ 
                    comboResetTimer = coolDownTime;
                    comboResetTimerActive = true;
                }

                if(currentStateInfo.IsName("LightAttack2") && currentStateInfo.normalizedTime >= 1 ){
                    canAttack = true;
                    curCombo++;
                    attackAnimator.SetBool("EAttack2",false); // Sets the animation state back to Idle
                    resetKnockback(); // Resets the knockback stats for this attack
                }
            }
            else if(curCombo == 2){
                attackAnimator.SetBool("EAttack3",true); // Sets the first combo attack in motion

                setKnockback(1f,4f); // Sets knockback stats for this attack

                //Since this is the third attack things are slightly different
                comboResetTimerActive = false; // No timer needed since we are now waiting on the animation to finish
            

                if(currentStateInfo.IsName("LightAttack3") && currentStateInfo.normalizedTime >= 1 ){
                    resetAttacks();
                    canAttack = true;
                    resetKnockback(); // Resets the knockback stats for this attack
                }
            }


            // Updates the knockback variables in the game instance

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
        
        if(currentAttackPressed == attackType.attackF)
{

    canAttack = false;
    
    if(curCombo == 0){
        attackAnimator.SetBool("FAttack1",true);

        setKnockback(1.5f,0f);

        if(!comboResetTimerActive){
            comboResetTimer = coolDownTime;
            comboResetTimerActive = true;
        }

        if(currentStateInfo.IsName("HeavyAttack1") && currentStateInfo.normalizedTime >= 1 ){
            Debug.Log("F attack combo 1 finished");
            canAttack = true;
            curCombo++;
            attackAnimator.SetBool("FAttack1",false);
            resetKnockback();
        }
    }
    else if(curCombo == 1){
        attackAnimator.SetBool("FAttack2",true);

        setKnockback(1.5f,0f);

        if(!comboResetTimerActive){
            comboResetTimer = coolDownTime;
            comboResetTimerActive = true;
        }

        if(currentStateInfo.IsName("HeavyAttack2") && currentStateInfo.normalizedTime >= 1 ){
            Debug.Log("F attack combo 2 finished");
            canAttack = true;
            curCombo++;
            attackAnimator.SetBool("FAttack2",false);
            resetKnockback();
        }
    }
    else if(curCombo == 2){
        attackAnimator.SetBool("FAttack3",true);

        setKnockback(0.5f,3f);

        if(!comboResetTimerActive){
            comboResetTimer = coolDownTime;
            comboResetTimerActive = true;
        }

        if(currentStateInfo.IsName("HeavyAttack3") && currentStateInfo.normalizedTime >= 1 ){
            Debug.Log("F attack combo 3 finished");
            canAttack = true;
            curCombo++;
            attackAnimator.SetBool("FAttack3",false);
            resetKnockback();
        }
    } else if(curCombo == 3) {
        attackAnimator.SetBool("FAttack4",true);

        setKnockback(4f,0.5f);

        comboResetTimerActive = false;

        if(currentStateInfo.IsName("HeavyAttack4") && currentStateInfo.normalizedTime >= 1 ){
            resetAttacks();
            canAttack = true;
            resetKnockback();
        }
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


    }

    void resetKnockback(){
        setKnockback(0f,0f); // Reset knockback
    }


    // Create more In the input manager with a different letter to indicate the different buttons used
    void OnAttackE(InputValue value){
        attackPressed = attackType.attackE;
    }

    void OnAttackF(InputValue value){
        attackPressed = attackType.attackF;
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
        attackPressed = 0;
        curCombo = 0;
        currentAttackPressed = attackType.none;
        //attackBoxCollider.enabled = false;
        attackAnimator.SetBool("EAttack1",false);
        attackAnimator.SetBool("EAttack2",false);
        attackAnimator.SetBool("EAttack3",false);
        
        attackAnimator.SetBool("FAttack1",false);
        attackAnimator.SetBool("FAttack2",false);
        attackAnimator.SetBool("FAttack3",false);
        attackAnimator.SetBool("FAttack4",false);
    }


    // Quick function for setting the knockback variables
    void setKnockback(float x , float y){
        knockbackX = x;
        knockbackY = y;
    }

    // Coroutine that is currently unused
    private IEnumerator attackingCooldown(){
        yield return new WaitForSeconds(coolDownTime);
        resetAttacks();
        canAttack = true;
    }
}
