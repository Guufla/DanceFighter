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

    public Slider p1offensive; // This is a reference to the player offensive slider that lets us easily call it from different scripts

    public bool isoffensive1 = false; // This is a bool that tells us if player 1 is in an offensive state or not



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

    public Slider p2offensive; // This is a reference to the player offensive slider that lets us easily call it from different scripts

    public bool isoffensive2 = false; // This is a bool that tells us if player 2 is in an offensive state or not


    //Offensive mode variables
    private float offensiveTimer1 = 0f;
    private float offensiveTimer2 = 0f;
    private float offensiveInterval = 1f;
    private float offensiveAmount = 50f;
    public int amount = 50;
    public int offensiveamount2;
    public int offensiveamount1;



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

        p1offensive.value = 0;
        p2offensive.value = 0;
        p1offensive.maxValue = 1000;
        p2offensive.maxValue = 1000;
        

    }

    void Update()
    {
        offensiveTimer1 += Time.deltaTime;
        offensiveTimer2 += Time.deltaTime;

        if (offensiveTimer1 >= offensiveInterval)
        {
            if (!isoffensive1)
            {
                IncreaseOffensiveSlider(p1offensive);
            }
            else
            {
                DecreaseOffensiveSlider(p1offensive, ref isoffensive1);
            }
            offensiveTimer1 = 0;
        }

        if (offensiveTimer2 >= offensiveInterval)
        {
            if (!isoffensive2)
            {
                IncreaseOffensiveSlider(p2offensive);
            }
            else
            {
                DecreaseOffensiveSlider(p2offensive, ref isoffensive2);
            }
            offensiveTimer2 = 0;
        }

        if (p1offensive.value == 1000)
        {
            isoffensive1 = true;
            offensiveamount1 = 45;
        }
        if (p2offensive.value == 1000)
        {
            isoffensive2 = true;
            offensiveamount2 = 45;
        }
    }

    private void DecreaseOffensiveSlider(Slider offensiveSlider, ref bool isoffensive)
    {
        offensiveSlider.value = Mathf.Max(offensiveSlider.value - offensiveAmount, 0);

        if (offensiveSlider.value == 0)
        {
            isoffensive = false;
        }
    }

    private void IncreaseOffensiveSlider(Slider offensiveSlider)
    {
        // when one second passes use this function to add 50 to the bar value but dont go over the max value which is 1000
        offensiveSlider.value = Mathf.Min(offensiveSlider.value + amount, offensiveSlider.maxValue);  
    
    }
    //When player 1 hits player 2 subtract 10 from player 2's health and update the slider
    public void Player1HitsPlayer2() 
    {   
        p2health -= 10;
        opponentHealth.value = p2health;
        if (!isoffensive1)
        {
            p1offensive.value = Mathf.Min(p1offensive.value + amount, p1offensive.maxValue);

        }
        
        //when p1 hits p2 add an extra boost to the offensive bar
        if (isoffensive1 && offensiveamount1 > 0)
        {
            p1offensive.value = Mathf.Min(p1offensive.value + offensiveamount1, p1offensive.maxValue);
            offensiveamount1 -= 15;

        }

        if (isoffensive2)
        {
            p2offensive.value = Mathf.Max(p2offensive.value - amount, 0);

        }
    }
    //When player 2 hits player 1 subtract 10 from player 2's health and update the slider
    public void Player2HitsPlayer1()
    {
        p1health -= 10;
        playerHealth.value = p1health; //when p2 hits p1 add an extra boost to the offensive bar
        if (!isoffensive2)
        {
            p2offensive.value = Mathf.Min(p2offensive.value + amount, p2offensive.maxValue);
        }
        
        if(isoffensive2 && offensiveamount2 > 0)
        {
            p2offensive.value = Mathf.Min(p2offensive.value + offensiveamount2, p2offensive.maxValue);
            offensiveamount2 -= 15;

        }
        if (isoffensive1)
        {
            p1offensive.value = Mathf.Max(p1offensive.value - amount, 0);
        }

    }
}
