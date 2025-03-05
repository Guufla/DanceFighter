using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxAnimationMethods : MonoBehaviour
{

    bool canInput; 

    public bool isAnimating; 

    public GameObject parentObject;

    void Start()
    {
        parentObject = transform.parent.gameObject;
        canInput = true;
        isAnimating = false;
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
}
