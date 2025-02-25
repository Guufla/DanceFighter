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

    public Slider playerHealthBar; // This is a reference to the player health slider that lets us easily call it from different scripts

    public int P1Health = 100;

    public Slider p1OffensiveBar; // This is a reference to the player offensive slider that lets us easily call it from different scripts

    public bool isOffensiveP1 = false; // This is a bool that tells us if player 1 is in an offensive state or not

    public bool p1Win = false;

    public static int p1WinCounter = 0;



    public bool player1IsOnGround; // Tells you if the player is on the ground or not

    public bool p1Aggro; // Tells if the player is in aggro mode


    // Player 2 is barely setup rn so none of this does anything yet except for the game object and the ground variable
    [Header("Player2")]

    public GameObject player2; // This is a game object reference to player 2 that lets us easily call it from different scripts

    // This is used to represent how much knockback any given attack does. It will usually be zero but while doing an attack it will switch around depending on the setKnockback() function.
    public float P2AttackKnockBackX; // When the knockback is set on an attack its x float will be held here. 

    public float P2AttackKnockBackY; // When the knockback is set on an attack its y float will be held here. 

    public Boolean stopP2Movement; 
    public Boolean stopP2YMovement;

    public Boolean canInputP2; 
    
    public Boolean isHitBoxAnimatingP2; 

    public bool P2Aggro; // Tells if the player is in aggro mode

    public Slider opponentHealth; // This is a reference to the player health slider that lets us easily call it from different scripts

    public int P2Health = 100;


    public Slider p2Offensive; // This is a reference to the player offensive slider that lets us easily call it from different scripts

    public bool isOffensiveP2 = false; // This is a bool that tells us if player 2 is in an offensive state or not

    public bool p2Win = false;

    public static int p2WinCounter = 0;

    public bool player2IsOnGround; // Tells you if the player is on the ground or not


    [Header("Offensive mode")]
    private float offensiveTimer1 = 0f;

    private float offensiveTimer2 = 0f;

    private float offensiveInterval = 1f;//every 1 second increase it by amount

    public int offensiveIncrease = 50; //amount to increase by

    public int offensiveDecrease = 50; //amount to decrease every second by when in offensive mode

    public int offensiveValueP1;

    public int offensiveValueP2; // amount to increase by when in offensive mode and you hit someone

    


    [Header("UI")]
    public TMP_Text winMessage; // Win message

    public Button restartButton; // Restart button

    public Button quitButton; // Quit button

    public TMP_Text countdownText; // Countdown text

    public bool isCountingDown; // Bool for when the game is counting down 

    public TMP_Text timerText;

    public float timerSeconds = 99f;

    public bool roundOver = false;

    public bool gameOver = false;
    


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
        playerHealthBar.maxValue = 100;
        playerHealthBar.value = P1Health;
        opponentHealth.value = P2Health;
        

        p1OffensiveBar.value = 0;
        p2Offensive.value = 0;
        p1OffensiveBar.maxValue = 1000;
        p2Offensive.maxValue = 1000;
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
            if (!isOffensiveP1)
            {
                IncreaseOffensiveSlider(p1OffensiveBar);
            }
            else
            {
                DecreaseOffensiveSlider(p1OffensiveBar, ref isOffensiveP1);
            }
            offensiveTimer1 = 0; // Reset the timer
        }

        if (offensiveTimer2 >= offensiveInterval) // what was before but just for player 2
        {
            if (!isOffensiveP2)
            {
                IncreaseOffensiveSlider(p2Offensive);
            }
            else
            {
                DecreaseOffensiveSlider(p2Offensive, ref isOffensiveP2);
            }
            offensiveTimer2 = 0;
        }

        // If the offensive bar is full then activate the offensive mode and reset the amount
        if (p1OffensiveBar.value == 1000)
        {
            isOffensiveP1 = true;
            offensiveValueP1 = 45;
        }
        if (p2Offensive.value == 1000)
        {
            isOffensiveP2 = true;
            offensiveValueP2 = 45;
        }
        if (P1Health <= 0)
        {
            Player2Win();
            
        }
        if (P2Health <= 0)
        {
            Player1Win();
            
        }
    }

    private void DecreaseOffensiveSlider(Slider offensiveSlider, ref bool isoffensive)
    {
        if (isCountingDown) return;
        //subtract the offensive amount from the bar value but dont go below 0
        offensiveSlider.value = Mathf.Max(offensiveSlider.value - offensiveDecrease, 0);

        if (offensiveSlider.value == 0) // once offensive bar is empty turn off offensive mode
        {
            isoffensive = false;
        }
    }

    private void IncreaseOffensiveSlider(Slider offensiveSlider)
    {
        if (!roundOver && !gameOver)
        {
            if (isCountingDown) return;
            // when one second passes use this function to add 50 to the bar value but dont go over the max value which is 1000
            offensiveSlider.value = Mathf.Min(offensiveSlider.value + offensiveIncrease, offensiveSlider.maxValue);

        }
        

    }
    //When player 1 hits player 2 subtract 10 from player 2's health and update the slider
    public void Player1HitsPlayer2()
    {
        if (isCountingDown) return;
        P2Health -= 10;
        opponentHealth.value = P2Health;
        if (!isOffensiveP1)
        {
            // add 50 to the offensive bar but dont go over the max value which is 1000
            p1OffensiveBar.value = Mathf.Min(p1OffensiveBar.value + offensiveIncrease, p1OffensiveBar.maxValue);

        }

        //when p1 hits p2 add an extra boost to the offensive bar
        if (isOffensiveP1 && offensiveValueP1 > 0)
        {
            // if you hit player while in offensive mode add 45 to the bar value but dont go over the max value which is 1000
            p1OffensiveBar.value = Mathf.Min(p1OffensiveBar.value + offensiveValueP1, p1OffensiveBar.maxValue);
            // minus the value by 15 so they cant have infinite offensive mode
            offensiveValueP1 -= 15;

        }

        if (isOffensiveP2)
        {
            //if player 2 if in offensive mode and you hit them decrease the offensive bar by 50 but dont go below 0
            p2Offensive.value = Mathf.Max(p2Offensive.value - offensiveIncrease, 0);

        }
    }
    //When player 2 hits player 1 subtract 10 from player 2's health and update the slider
    public void Player2HitsPlayer1()
    {
        if (isCountingDown) return;
        P1Health -= 10;
        playerHealthBar.value = P1Health; //when p2 hits p1 add an extra boost to the offensive bar
        if (!isOffensiveP2)
        {
            p2Offensive.value = Mathf.Min(p2Offensive.value + offensiveIncrease, p2Offensive.maxValue);
        }

        if (isOffensiveP2 && offensiveValueP2 > 0)
        {
            p2Offensive.value = Mathf.Min(p2Offensive.value + offensiveValueP2, p2Offensive.maxValue);
            offensiveValueP2 -= 15;

        }
        if (isOffensiveP1)
        {
            p1OffensiveBar.value = Mathf.Max(p1OffensiveBar.value - offensiveIncrease, 0);
        }

    }

    public void Player1Win()
    {
        if (!p1Win)
        {
            // When p1 wins makes the bool true and add 1 to its round score and display text
            roundOver = true;
            p1Win = true;
            p1WinCounter += 1;
            winMessage.text = "Player 1 Wins!";
            winMessage.gameObject.SetActive(true);

            if (p1WinCounter >= 2) // When player 1 wins 2 rounds game is over 
            {
                gameOver = true;
                winMessage.text = "Player 1 Wins";
                winMessage.gameObject.SetActive(true);
                quitButton.gameObject.SetActive(true);
                restartButton.gameObject.SetActive(true);
            }
            else
            {
                StartCoroutine(RestartMatch(3f));
            }
        }
    }

    public void Player2Win()
    {
        if (!p2Win)
        {
            // When p2 wins makes the bool true and add 1 to its round score and display text
            roundOver = true;
            p2Win = true;
            p2WinCounter += 1;
            winMessage.text = "Player 2 Wins!";
            winMessage.gameObject.SetActive(true);

            if (p2WinCounter >= 2) // When player 2 wins 2 rounds game is over 
            {
                gameOver = true;
                winMessage.text = "Player 2 Wins";
                winMessage.gameObject.SetActive(true);
                quitButton.gameObject.SetActive(true);
                restartButton.gameObject.SetActive(true);
            }
            else
            {
                StartCoroutine(RestartMatch(3f));
            }
        }
    }

    private IEnumerator RestartMatch(float waitTime) // Makes sure match doesnt restart automatically
    {
        yield return new WaitForSeconds(waitTime);
        yield return new WaitForSeconds(waitTime);

        // Hide the win message
        winMessage.gameObject.SetActive(false);

        // Reset Timer
        timerSeconds = 99f;
        countdownText.text = "99";


        // Reset player health
        P1Health = 100;
        P2Health = 100;
        playerHealthBar.value = P1Health;
        opponentHealth.value = P2Health;

        // Reset offensive sliders
        p1OffensiveBar.value = 0;
        p2Offensive.value = 0;
        isOffensiveP1 = false;
        isOffensiveP2 = false;

        // Reset win booleans
        p1Win = false;
        p2Win = false;
        roundOver = false;
        gameOver = false;

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
        countdownText.text = "99";
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
        if (timerSeconds <= 0)
        {
            if (P1Health > P2Health)
            {
                Player1Win();
            }
            else if (P2Health > P1Health)
            {
                Player2Win();

            }
            else
            {
                winMessage.text = "Draw";
                winMessage.gameObject.SetActive(true);
                StartCoroutine(RestartMatch(2f));

            }
        
        
        }
    }
    
}
