using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxAnimationMethods : MonoBehaviour
{

    bool canInput; 

    GameObject parentObject;

    void Start()
    {
        parentObject = transform.parent.gameObject;
        canInput = true;
    }

    void Update()
    {
        if(parentObject.CompareTag("Player1")){
            GameManager.Instance.canInputP1 = canInput;
        }
        else{
            GameManager.Instance.canInputP2 = canInput;
        }
    }

    // Start is called before the first frame update
    void allowInput(){
        canInput = true;
    }
    void stopInput(){
        canInput = false;
    }
}
