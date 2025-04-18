using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerDirections : MonoBehaviour
{
    GameObject oppositePlayer;

    Transform curTrans; // Current position

    Transform opPlayerTrans; // opposite player position

    public bool isFacingRight;
    public bool isFacingLeft;

    string currentPlayer;

    bool isOnGround;

    bool isAttacking;

    // Start is called before the first frame update
    void Start()
    {

        currentPlayer = gameObject.tag.ToString();
        
        if(currentPlayer == "Player1"){
            isOnGround = GameManager.Instance.player1IsOnGround;

            oppositePlayer = GameManager.Instance.player2;
        }
        else if(currentPlayer == "Player2"){
            isOnGround = GameManager.Instance.player2IsOnGround;

            oppositePlayer = GameManager.Instance.player1;
        }


        curTrans = GetComponent<Transform>();
        opPlayerTrans = oppositePlayer.GetComponent<Transform>();
        
        isFacingLeft = false;
        isFacingRight = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(currentPlayer == "Player1"){
            isOnGround = GameManager.Instance.player1IsOnGround;

            isAttacking = GameManager.Instance.isP1Attacking;

            oppositePlayer = GameManager.Instance.player2;
        }
        else if(currentPlayer == "Player2"){
            isOnGround = GameManager.Instance.player2IsOnGround;

            isAttacking = GameManager.Instance.isP2Attacking;

            oppositePlayer = GameManager.Instance.player1;
        }


        if(isOnGround && curTrans.position.x < opPlayerTrans.position.x && !isAttacking){ 
            transform.localScale = new Vector2(1f,1f);
            isFacingRight = true;
            isFacingLeft = false;
        }
        else if(isOnGround && curTrans.position.x > opPlayerTrans.position.x && !isAttacking){
            transform.localScale = new Vector2(-1f,1f);
            isFacingRight = false;
            isFacingLeft = true;
        }
    }
}
