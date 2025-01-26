using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
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

        

        Debug.DrawRay(transform.position, new UnityEngine.Vector2(0,-1.12f));

        RaycastHit2D hit = Physics2D.Raycast(transform.position, UnityEngine.Vector2.down, 1.12f, LayerMask.GetMask("Environment"));

        if (hit.collider != null) {
            playerGroundCheck = true;
        }
        else{
            playerGroundCheck = false;
        }

    }
}