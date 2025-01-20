using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundCheck : MonoBehaviour
{
    bool playerGroundCheck;

    bool managerGroundCheck;


    string currentPlayer;

    // Start is called before the first frame update
    void Start()
    {
        playerGroundCheck = false;
        currentPlayer = gameObject.tag.ToString();
        
    }

    // Update is called once per frame
    void Update()
    {
        if(currentPlayer == "Player1"){
            GameManager.Instance.player1IsOnGround = playerGroundCheck;
        }
        else if(currentPlayer == "Player2"){
            GameManager.Instance.player2IsOnGround = playerGroundCheck;
        }


    }

    void OnTriggerStay2D(Collider2D other)
    {
        if(other.CompareTag("Ground")){
            playerGroundCheck = true;
        }
    }


    void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Ground")){
            playerGroundCheck = false;
        }
    }
}
