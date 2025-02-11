using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxAnimationMethods : MonoBehaviour
{

    bool canInput; 

    bool isAnimating; 

    GameObject parentObject;

    void Start()
    {
        parentObject = transform.parent.gameObject;
        canInput = true;
        isAnimating = true;
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
    void allowInput(){
        canInput = true;
    }
    void stopInput(){
        // Both input and animating should be set at the beginning
        canInput = false;
        isAnimating = true;
    }

    void animationEnd(){
        isAnimating = false;
    }
    void stopAttack(){
        isAnimating = true;
    }
}
