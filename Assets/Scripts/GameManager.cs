using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

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

    public Boolean canInputP1; 

    public Boolean isHitBoxAnimatingP1; 

    public Slider playerHealth; // This is a reference to the player health slider that lets us easily call it from different scripts

    public int p1health = 100;

    public Slider p1offensive; // This is a reference to the player offensive slider that lets us easily call it from different scripts

    public bool isoffensive1 = false; // This is a bool that tells us if player 1 is in an offensive state or not

    public bool p1Win = false;

    public static int p1WinCounter = 0;



    public bool player1IsOnGround; // Tells you if the player is on the ground or not

    public bool P1Aggro; // Tells if the player is in aggro mode


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

    public bool p2Win = false;

    public static int p2WinCounter = 0;


    [Header("Offensive mode")]
    private float offensiveTimer1 = 0f;

    private float offensiveTimer2 = 0f;

    private float offensiveInterval = 1f;//every 1 second increase it by amount

    public int amount = 50; //amount to increase by

    public int offensiveAmount = 50; //amount to decrease every second by when in offensive mode

    public int offensiveamount2; // amount to increase by when in offensive mode and you hit someone

    public int offensiveamount1;


    [Header("UI")]
    public TMP_Text winMessage; // Win message

    public Button restartButton; // Restart button

    public Button quitButton; // Quit button

    public TMP_Text countdownText; // Countdown text

    public bool isCountingDown; // Bool for when the game is counting down 

    public TMP_Text timerText;

    public float timerSeconds = 99f;
    
    public Boolean stopP2Movement; 
    public Boolean stopP2YMovement;

    public Boolean canInputP2; 
    
    public Boolean isHitBoxAnimatingP2; 

    public bool player2IsOnGround; // Tells you if the player is on the ground or not

    public bool P2Aggro; // Tells if the player is in aggro mode


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
            Debug.LogError("GameManager is already in the scene");
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(this); 
        }
        
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
        winMessage.gameObject.SetActive(false);
        quitButton.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);

        StartCoroutine(StartRoundCountdown(3)); // Start the countdown

    }

    void Update()
    {
        if (isCountingDown) return;
        offensiveTimer1 += Time.deltaTime; // Calulate the time that has passed
        offensiveTimer2 += Time.deltaTime;

        if (offensiveTimer1 >= offensiveInterval) // If the time that has passed is greater than the interval (1sec) then do the following
        {
            if (!isoffensive1)
            {
                IncreaseOffensiveSlider(p1offensive);
            }
            else
            {
                DecreaseOffensiveSlider(p1offensive, ref isoffensive1);
            }
            offensiveTimer1 = 0; // Reset the timer
        }

        if (offensiveTimer2 >= offensiveInterval) // what was before but just for player 2
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

        // If the offensive bar is full then activate the offensive mode and reset the amount
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
        if (p1health <= 0)
        {
            Player2Win();
            
        }
        if (p2health <= 0)
        {
            Player1Win();
            
        }
    }

    private void DecreaseOffensiveSlider(Slider offensiveSlider, ref bool isoffensive)
    {
        if (isCountingDown) return;
        //subtract the offensive amount from the bar value but dont go below 0
        offensiveSlider.value = Mathf.Max(offensiveSlider.value - offensiveAmount, 0);

        if (offensiveSlider.value == 0) // once offensive bar is empty turn off offensive mode
        {
            isoffensive = false;
        }
    }

    private void IncreaseOffensiveSlider(Slider offensiveSlider)
    {
        if (isCountingDown) return;
        // when one second passes use this function to add 50 to the bar value but dont go over the max value which is 1000
        offensiveSlider.value = Mathf.Min(offensiveSlider.value + amount, offensiveSlider.maxValue);

    }
    //When player 1 hits player 2 subtract 10 from player 2's health and update the slider
    public void Player1HitsPlayer2()
    {
        if (isCountingDown) return;
        p2health -= 10;
        opponentHealth.value = p2health;
        if (!isoffensive1)
        {
            // add 50 to the offensive bar but dont go over the max value which is 1000
            p1offensive.value = Mathf.Min(p1offensive.value + amount, p1offensive.maxValue);

        }

        //when p1 hits p2 add an extra boost to the offensive bar
        if (isoffensive1 && offensiveamount1 > 0)
        {
            // if you hit player while in offensive mode add 45 to the bar value but dont go over the max value which is 1000
            p1offensive.value = Mathf.Min(p1offensive.value + offensiveamount1, p1offensive.maxValue);
            // minus the value by 15 so they cant have infinite offensive mode
            offensiveamount1 -= 15;

        }

        if (isoffensive2)
        {
            //if player 2 if in offensive mode and you hit them decrease the offensive bar by 50 but dont go below 0
            p2offensive.value = Mathf.Max(p2offensive.value - amount, 0);

        }
    }
    //When player 2 hits player 1 subtract 10 from player 2's health and update the slider
    public void Player2HitsPlayer1()
    {
        if (isCountingDown) return;
        p1health -= 10;
        playerHealth.value = p1health; //when p2 hits p1 add an extra boost to the offensive bar
        if (!isoffensive2)
        {
            p2offensive.value = Mathf.Min(p2offensive.value + amount, p2offensive.maxValue);
        }

        if (isoffensive2 && offensiveamount2 > 0)
        {
            p2offensive.value = Mathf.Min(p2offensive.value + offensiveamount2, p2offensive.maxValue);
            offensiveamount2 -= 15;

        }
        if (isoffensive1)
        {
            p1offensive.value = Mathf.Max(p1offensive.value - amount, 0);
        }

    }

    public void Player1Win()
    {
        if (!p1Win)
        {
            // When p1 wins makes the bool true and add 1 to its round score and display text
            p1Win = true;
            p1WinCounter += 1;
            winMessage.text = "Player 1 Wins!";
            winMessage.gameObject.SetActive(true);
            StartCoroutine(RestartMatch(3f)); 
        }
        if (p1WinCounter >= 2) // When player 1 wins 2 rounds game is over 
        {
            winMessage.text = "Player 1 Wins!";
            winMessage.gameObject.SetActive(true);
            quitButton.gameObject.SetActive(true);
            restartButton.gameObject.SetActive(true);
        }
    }

    public void Player2Win()
    {
        if (!p2Win)
        {
            // When p2 wins makes the bool true and add 1 to its round score and display text
            p2Win = true;
            p2WinCounter += 1;
            winMessage.text = "Player 2 Wins!";
            winMessage.gameObject.SetActive(true);
            StartCoroutine(RestartMatch(3f));
        }
        if (p2WinCounter >= 2) // When player 2 wins 2 rounds game is over 
        {
            winMessage.text = "Player 2 Wins!";
            winMessage.gameObject.SetActive(true);
            quitButton.gameObject.SetActive(true);
            restartButton.gameObject.SetActive(true);
        }
    }

    private IEnumerator RestartMatch(float waitTime) // Makes sure match doesnt restart automatically
    {
        yield return new WaitForSeconds(waitTime);
        yield return new WaitForSeconds(waitTime);

        // Hide the win message
        winMessage.gameObject.SetActive(false);

        // Reset player health
        p1health = 100;
        p2health = 100;
        playerHealth.value = p1health;
        opponentHealth.value = p2health;

        // Reset offensive sliders
        p1offensive.value = 0;
        p2offensive.value = 0;
        isoffensive1 = false;
        isoffensive2 = false;

        // Reset win booleans
        p1Win = false;
        p2Win = false;

        // Reposition players to their initial positions
        player1.transform.position = new Vector2(-4, -3);
        player2.transform.position = new Vector2(4, -3);

        StartCoroutine(StartRoundCountdown(3));
    }

    public void RestartGame() // When restarting the whole game set the win counters to 0
    {
        // Reset win counters
        p1WinCounter = 0;
        p2WinCounter = 0;

        // Hide buttons
        quitButton.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);

        // Reset the match
        StartCoroutine(RestartMatch(2f));
    }

    private IEnumerator StartRoundCountdown(int countdownTime) // Starts the countdown for the round
    {
        timerSeconds = 99f;
        isCountingDown = true;
        countdownText.gameObject.SetActive(true);
        while (countdownTime > 0)
        {
            countdownText.text = countdownTime.ToString();
            yield return new WaitForSeconds(1f);
            countdownTime--;
        }
        countdownText.text = "GO!";
        yield return new WaitForSeconds(1f);
        countdownText.gameObject.SetActive(false);
        isCountingDown = false;
        StartCoroutine(CountdownTimer());
    }

    private IEnumerator CountdownTimer()
    {
        
        while (timerSeconds > 0)
        {
            if (isCountingDown || p1Win || p2Win)
            {
                yield break; // Exit the coroutine if the game is counting down or if someone has won
            }

            timerText.text = timerSeconds.ToString("F0"); // Updates the text to display seconds value
            yield return new WaitForSeconds(1f); // After 1 second subtract 1 from the timer
            timerSeconds--;
        }
        timerText.text = "0";



    }
    
}
