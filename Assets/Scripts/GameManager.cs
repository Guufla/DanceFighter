using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;


    [Header("Player1")]


    public GameObject player1;

    public float player1MaxHP;

    public float player1HP;

    public float P1AttackKnockBackX;

    public float P1AttackKnockBackY;


    public bool player1IsOnGround;



    [Header("Player2")]



    public GameObject player2;

    public float player2MaxHP;

    public float player2HP;

    public float P2AttackKnockBackX;

    public float P2AttackKnockBackY;


    public bool player2IsOnGround;

    public static GameManager Instance
    {
        get
        {
            if(_instance == null)
            {
                Debug.LogError("GameManager is Null");
            }
            return _instance;
        }
        
    }
    private void Awake(){
        if(_instance){
            Destroy(gameObject);
        }
        else{
            _instance = this;
        }
        DontDestroyOnLoad(this);
    }

    void Start(){
        player1IsOnGround = false;
        player2IsOnGround = false;
    }
}
