using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxAnimationMethods : MonoBehaviour
{

    bool canInput; 

    public bool isAnimating; 
    
    private bool isNextAttackQueued;

    [SerializeField] private GameObject parentObject;

    private Animator animator;
    
    private PlayerAttack playerAttack;

    void Start()
    {
        parentObject = transform.parent.gameObject;
        canInput = true;
        isAnimating = false;
        animator = this.GetComponent<Animator>();
        playerAttack = parentObject.GetComponent<PlayerAttack>();
    }

    void Update()
    {
        if(parentObject.CompareTag("Player1")){
            GameManager.Instance.canInputP1 = canInput;
            GameManager.Instance.isHitBoxAnimatingP1 = isAnimating;
        }
        else{
            GameManager.Instance.canInputP2 = canInput;
            GameManager.Instance.isHitBoxAnimatingP2 = isAnimating;
        }
    }

    // Start is called before the first frame update
    public void allowInput(){
        canInput = true;
    }
    public void stopInput(){
        // Both input and animating should be set at the beginning
        canInput = false;
        isAnimating = true;
    }

    public void animationEnd(){
        isAnimating = false;
    }
    public void stopAttack(){
        isAnimating = true;
    }
    
    public void QueueNextAttack(){
        isNextAttackQueued = true;
    }
    
    private void TransitionToNextState()
    {
        Debug.Log(animator == null);
        Debug.Log(isNextAttackQueued);
    
        //Debug.Log(animator != null && isNextAttackQueued);
        
        if(animator != null && isNextAttackQueued){
            animator.SetTrigger("NextState");
            
            if(playerAttack != null){
                playerAttack.completeAnimation(1, "RAttack1", "MediumAttack1", false);
            }
            
            isNextAttackQueued = false;
        }else{
            Debug.Log("Animator is null");
        }
    }
}
