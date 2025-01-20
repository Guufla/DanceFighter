using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerDirections : MonoBehaviour
{
    GameObject oppositePlayer;

    Transform curTrans; // Current position

    Transform opPlayerTrans; // opposite player position



    string currentPlayer;

    bool isOnGround;

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
    }

    // Update is called once per frame
    void Update()
    {
        if(currentPlayer == "Player1"){
            isOnGround = GameManager.Instance.player1IsOnGround;

            oppositePlayer = GameManager.Instance.player2;
        }
        else if(currentPlayer == "Player2"){
            isOnGround = GameManager.Instance.player2IsOnGround;

            oppositePlayer = GameManager.Instance.player1;
        }


        if(isOnGround && curTrans.position.x < opPlayerTrans.position.x ){
            transform.localScale = new Vector2(1f,1f);
        }
        else if(isOnGround && curTrans.position.x > opPlayerTrans.position.x ){
            transform.localScale = new Vector2(-1f,1f);
        }
    }
}
