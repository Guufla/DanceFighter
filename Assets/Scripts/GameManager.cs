using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;



    [Header("Player1")]

    public GameObject player1; // This is a game object reference to player 1 that lets us easily call it from different scripts

    // This is used to represent how much knockback any given attack does. It will usually be zero but while doing an attack it will switch around depending on the setKnockback() function.
    public float P1AttackKnockBackX; // When the knockback is set on an attack its x float will be held here. 

    public float P1AttackKnockBackY; // When the knockback is set on an attack its y float will be held here.

    public Boolean stopP1Movement; 
    public Boolean stopP1YMovement; 


    public bool player1IsOnGround; // Tells you if the player is on the ground or not


    // Player 2 is barely setup rn so none of this does anything yet except for the game object and the ground variable
    [Header("Player2")]

    public GameObject player2; // This is a game object reference to player 2 that lets us easily call it from different scripts

    // This is used to represent how much knockback any given attack does. It will usually be zero but while doing an attack it will switch around depending on the setKnockback() function.
    public float P2AttackKnockBackX; // When the knockback is set on an attack its x float will be held here. 

    public float P2AttackKnockBackY; // When the knockback is set on an attack its y float will be held here. 

    public Boolean stopP2Movement; 
    public Boolean stopP2YMovement;
    

    public bool player2IsOnGround; // Tells you if the player is on the ground or not


    // Used to make the game manager. Doesnt really need to be edited
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
    private void Awake()
    {
        if(_instance)
        {
            Debug.LogError("GameManager is already in the scene");
            Destroy(gameObject);
        }
        else{
            _instance = this;
            DontDestroyOnLoad(this); 
        }
        
    }

    void Start()
    {
        // Sets the ground variables to false by default
        player1IsOnGround = false;
        player2IsOnGround = false;
    }
}
