using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;



    [Header("Player1")]

    public GameObject player1; // This is a game object reference to player 1 that lets us easily call it from different scripts

    // This is used to represent how much knockback any given attack does. It will usually be zero but while doing an attack it will switch around depending on the setKnockback() function.
    public float P1AttackKnockBackX; // When the knockback is set on an attack its x float will be held here. 

    public float P1AttackKnockBackY; // When the knockback is set on an attack its y float will be held here. 

    public Slider playerHealth; // This is a reference to the player health slider that lets us easily call it from different scripts

    public int p1health = 100;



    public bool player1IsOnGround; // Tells you if the player is on the ground or not


    // Player 2 is barely setup rn so none of this does anything yet except for the game object and the ground variable
    [Header("Player2")]

    public GameObject player2; // This is a game object reference to player 2 that lets us easily call it from different scripts

    // This is used to represent how much knockback any given attack does. It will usually be zero but while doing an attack it will switch around depending on the setKnockback() function.
    public float P2AttackKnockBackX; // When the knockback is set on an attack its x float will be held here. 

    public float P2AttackKnockBackY; // When the knockback is set on an attack its y float will be held here. 

    public Slider opponentHealth; // This is a reference to the player health slider that lets us easily call it from different scripts

    public int p2health = 100;


    public bool player2IsOnGround; // Tells you if the player is on the ground or not



    // Used to make the game manager. Doesnt really need to be edited
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("GameManager is Null");
            }
            return _instance;
        }

    }
    private void Awake()
    {
        if (_instance)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
        DontDestroyOnLoad(this);
    }

    void Start()
    {
        // Sets the ground variables to false by default
        player1IsOnGround = false;
        player2IsOnGround = false;

        // Sets the health bar values to the player health variables
        // Max health is 100 for now
        // MAKE SURE TO SET MAX VALUES FIRST
        opponentHealth.maxValue = 100;
        playerHealth.maxValue = 100;
        playerHealth.value = p1health;
        opponentHealth.value = p2health;
        
    }

    void Update()
    {

    }
    //When player 1 hits player 2 subtract 10 from player 2's health and update the slider
    public void Player1HitsPlayer2() 
    {   
        p2health -= 10;
        opponentHealth.value = p2health;
    }
    //When player 2 hits player 1 subtract 10 from player 2's health and update the slider
    public void Player2HitsPlayer1()
    {
        p1health -= 10;
        playerHealth.value = p1health;
    }
}
