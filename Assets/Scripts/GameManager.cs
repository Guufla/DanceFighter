using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public float hitLagTime; 
    public bool isWaiting;

    public int curPlayerCount;

    public bool lockGameTillEnoughPlayers;

    public bool isGameStarted;

    public bool disablePlayerInputs;


    [Header("Player1")]

    public GameObject player1; // This is a game object reference to player 1 that lets us easily call it from different scripts

    // This is used to represent how much knockback any given attack does. It will usually be zero but while doing an attack it will switch around depending on the setKnockback() function.
    public float P1AttackKnockBackX; // When the knockback is set on an attack its x float will be held here. 

    public float P1AttackKnockBackY; // When the knockback is set on an attack its y float will be held here.

    public Boolean stopP1Movement; //Use UpdateStopMovement instead
    public Boolean stopP1YMovement; //Use UpdateStopMovementY instead

    public Boolean canInputP1; 

    public Boolean isHitBoxAnimatingP1; 

    public Slider playerHealthBar; // This is a reference to the player health slider that lets us easily call it from different scripts

    public float P1Health = 1000;

    public Slider p1OffensiveBar; // This is a reference to the player offensive slider that lets us easily call it from different scripts

    public static int p1WinCounter = 0;



    public bool player1IsOnGround; // Tells you if the player is on the ground or not

    public bool p1Aggro; // Tells if the player is in aggro mode

    public bool isP1Hitstun; // 

    public float p1HitstunSetTime; //

    public float p1HitstunTime; // 

    public float p1AttackDamage;

    public bool p1IsBlocking;

    public bool p1IsParrying;

    


    // Player 2 is barely setup rn so none of this does anything yet except for the game object and the ground variable
    [Header("Player2")]

    public GameObject player2; // This is a game object reference to player 2 that lets us easily call it from different scripts

    // This is used to represent how much knockback any given attack does. It will usually be zero but while doing an attack it will switch around depending on the setKnockback() function.
    public float P2AttackKnockBackX; // When the knockback is set on an attack its x float will be held here. 

    public float P2AttackKnockBackY; // When the knockback is set on an attack its y float will be held here. 

    public Boolean stopP2Movement; //Use UpdateStopMovement instead
    public Boolean stopP2YMovement; //Use UpdateStopMovementY instead

    public Boolean canInputP2; 
    
    public Boolean isHitBoxAnimatingP2; 

    public bool P2Aggro; // Tells if the player is in aggro mode

    public Slider opponentHealth; // This is a reference to the player health slider that lets us easily call it from different scripts

    public float P2Health = 1000;

    public Slider p2Offensive; // This is a reference to the player offensive slider that lets us easily call it from different scripts

    public static int p2WinCounter = 0;

    public bool player2IsOnGround; // Tells you if the player is on the ground or not

    public bool isP2Hitstun; // 
    
    public float p2HitstunSetTime; //

    public float p2HitstunTime; // 

    public float p2AttackDamage;

    public bool p2IsBlocking;

    public bool p2IsParrying;

    [Header("Offensive mode")]

    public float maxOffensiveBarValue;

    private float offensiveTimer1 = 0f;

    private float offensiveTimer2 = 0f;

    private float offensiveInterval = 1f;//every 1 second increase it by amount

    public int offensiveIncrease = 50; //amount to increase by

    public int offensiveDecrease = 50; //amount to decrease every second by when in offensive mode

    public int offensiveValueP1;

    public int offensiveValueP2; // amount to increase by when in offensive mode and you hit someone

    public bool isOffensiveP2 = false; // This is a bool that tells us if player 2 is in an offensive state or not

    public bool isOffensiveP1 = false; // This is a bool that tells us if player 1 is in an offensive state or not


    [Header("Text")]

    public Image controllerConnectScreen;

    public TMP_Text player1Joined;

    public TMP_Text player2Joined;

    public TMP_Text winMessage; // Win message

    public TMP_Text countdownText; // Countdown text

    public TMP_Text timerText; // Timer for round text 

    public float timerSeconds = 99f;

    [Header("Winboxes")]

    public Image emptyBox1P1;

    public Image emptyBox2P1;

    public Image winBox1P1;

    public Image winBox2P1;

    public Image emptyBox1P2;

    public Image emptyBox2P2;

    public Image winBox1P2;

    public Image winBox2P2;


    [Header("Buttons")]

    public Button restartButton; // Restart button

    public Button quitButton; // Quit button


    [Header("Bools")]

    public bool isCountingDown; // Bool for when the game is counting down 

    public bool roundOver = false;

    public bool gameOver = false;

    public bool p1Win = false;

    public bool p2Win = false;
    //Variables for this script
    private Coroutine knockbackCoroutineP1; // Coroutine for knockback timer
    private Coroutine knockbackCoroutineP2;

    // Array to hold the knockback frozen state for each script and each player
    // First is player, next is script
    // true means frozen
    private bool[][] knockbackFrozen = new bool[2][];
    private bool[][] knockbackFrozenY = new bool[2][];


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

        disablePlayerInputs = true;

        curPlayerCount = 0;

        // Sets the health bar values to the player health variables
        // Max health is 100 for now
        // MAKE SURE TO SET MAX VALUES FIRST
        opponentHealth.maxValue = P2Health;
        playerHealthBar.maxValue = P1Health;
        playerHealthBar.value = P1Health;
        opponentHealth.value = P2Health;
        

        p1OffensiveBar.value = 0;
        p2Offensive.value = 0;
        p1OffensiveBar.maxValue = maxOffensiveBarValue;
        p2Offensive.maxValue = maxOffensiveBarValue;
        winMessage.gameObject.SetActive(false);
        quitButton.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);
        emptyBox1P1.gameObject.SetActive(true);
        emptyBox2P1.gameObject.SetActive(true);
        winBox1P1.gameObject.SetActive(false);
        winBox2P1.gameObject.SetActive(false);
        emptyBox1P2.gameObject.SetActive(true);
        emptyBox2P2.gameObject.SetActive(true);
        winBox1P2.gameObject.SetActive(false);
        winBox2P2.gameObject.SetActive(false);

        countdownText.gameObject.SetActive(false);

        controllerConnectScreen.gameObject.SetActive(true);
        player1Joined.gameObject.SetActive(false);
        player2Joined.gameObject.SetActive(false);

        isP1Hitstun = false; // 

        p1HitstunSetTime = 0; // 

        p1HitstunTime = 0; // 

        isP2Hitstun = false; // 

        p2HitstunSetTime = 0; // 

        p2HitstunTime = 0; // 

        isWaiting = false;

        isGameStarted = false;

        knockbackFrozen[0] = new bool[2];
        knockbackFrozen[1] = new bool[2];
        knockbackFrozenY[0] = new bool[2];
        knockbackFrozenY[1] = new bool[2];

        if(lockGameTillEnoughPlayers){
            disablePlayerInputs = true;
            controllerConnectScreen.gameObject.SetActive(true);
        }
        else{
            controllerConnectScreen.gameObject.SetActive(false);
            disablePlayerInputs = true;
            countdownText.gameObject.SetActive(true);
            StartCoroutine(StartRoundCountdown(3)); // Start the countdown
        }

    }

    void Update()
    {
        if(!isGameStarted && controllerConnectScreen.gameObject.activeSelf)
        {
            if(curPlayerCount == 0){
                player1Joined.gameObject.SetActive(false);
                player2Joined.gameObject.SetActive(false);
            }

            else if(curPlayerCount == 1){
                player1Joined.gameObject.SetActive(true);
            }
            
            else if(curPlayerCount == 2 && !isCountingDown)
            {
                player1Joined.gameObject.SetActive(true);
                player2Joined.gameObject.SetActive(true);

                StartCoroutine(bufferForControllerScreen(1f));
            }
        }

        if (isCountingDown || !isGameStarted) return;
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
                AudioManager.Instance.StateChange();
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
                AudioManager.Instance.StateChange();
                DecreaseOffensiveSlider(p2Offensive, ref isOffensiveP2);
            }
            offensiveTimer2 = 0;
        }

        // If the offensive bar is full then activate the offensive mode and reset the amount
        if (p1OffensiveBar.value == maxOffensiveBarValue)
        {
            isOffensiveP1 = true;
            offensiveValueP1 = 45;
        }
        if (p2Offensive.value == maxOffensiveBarValue)
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
    public void hitLagCheck(){
        if(!isWaiting){
            Time.timeScale = 0f;
            StartCoroutine(Wait(hitLagTime));
        }
    }

    IEnumerator Wait(float duration){
        isWaiting = true;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1f;
        isWaiting = false;
    }

    private void DecreaseOffensiveSlider(Slider offensiveSlider, ref bool isoffensive)
    {
        if (!roundOver && !gameOver)
        {
            if (isCountingDown) return;
            //subtract the offensive amount from the bar value but dont go below 0
            offensiveSlider.value = Mathf.Max(offensiveSlider.value - offensiveDecrease, 0);

            if (offensiveSlider.value == 0) // once offensive bar is empty turn off offensive mode
            {
                isoffensive = false;
            }
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

        //Debug.Log("Player 1 hits Player 2");

        P2Health -= p1AttackDamage;
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

        //stops other players movement for knockback
        UpdateStopMovement(2, true, 1);
        UpdateStopMovementY(2, false, 1);

        //if timer is already running the stop it
        if(knockbackCoroutineP1 != null)
        {
            StopCoroutine(knockbackCoroutineP1);
        }

        //start the knockback timer
        knockbackCoroutineP1 = StartCoroutine(KnockbackTimer(2));
        
        // hit effect
        EffectManager.Instance.OnPlayerHit(EffectManager.Instance.p2);
    }
    //When player 2 hits player 1 subtract 10 from player 2's health and update the slider
    public void Player2HitsPlayer1()
    {
        if (isCountingDown) return;

        //Debug.Log("Player 2 hits Player 1");

        P1Health -= p2AttackDamage;
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

        //stops other players movement for knockback
        UpdateStopMovement(1, true, 1);
        UpdateStopMovementY(1, false, 1);

        if(knockbackCoroutineP2 != null)
        {
            StopCoroutine(knockbackCoroutineP2);
        }

        //start the knockback timer
        knockbackCoroutineP2 = StartCoroutine(KnockbackTimer(1));
        
        // hit effect
        EffectManager.Instance.OnPlayerHit(EffectManager.Instance.p1);
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
            winBox1P1.gameObject.SetActive(true);

            if (p1WinCounter >= 2) // When player 1 wins 2 rounds game is over 
            {
                gameOver = true;
                winBox2P1.gameObject.SetActive(true);
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
            winBox1P2.gameObject.SetActive(true);

            if (p2WinCounter >= 2) // When player 2 wins 2 rounds game is over 
            {
                gameOver = true;
                winBox2P2.gameObject.SetActive(true);
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
        P1Health = playerHealthBar.maxValue;
        P2Health = playerHealthBar.maxValue;
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

        // Hide buttons and winboxes
        quitButton.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);
        emptyBox1P1.gameObject.SetActive(true);
        emptyBox2P1.gameObject.SetActive(true);
        winBox1P1.gameObject.SetActive(false);
        winBox2P1.gameObject.SetActive(false);
        emptyBox1P2.gameObject.SetActive(true);
        emptyBox2P2.gameObject.SetActive(true);
        winBox1P2.gameObject.SetActive(false);
        winBox2P2.gameObject.SetActive(false);

        // Reset the match
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
    private IEnumerator bufferForControllerScreen(float bufferTime){
        yield return new WaitForSeconds(bufferTime);

        player1Joined.gameObject.SetActive(false);
        player2Joined.gameObject.SetActive(false);
        controllerConnectScreen.gameObject.SetActive(false);

        countdownText.gameObject.SetActive(true);
        StartCoroutine(StartRoundCountdown(3)); // Start the countdown
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
        disablePlayerInputs = false;
        countdownText.text = "GO!";
        yield return new WaitForSeconds(1f);
        countdownText.gameObject.SetActive(false);
        isCountingDown = false;
        if(!isGameStarted){
            StartCoroutine(CountdownTimer());
        }
        isGameStarted = true;
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

    //unfreezes the player after being hit
    private IEnumerator KnockbackTimer(int player)
    {
        //Debug.Log("Knockback timer started for player " + player);
        //yield return new WaitForSeconds(1f); // Wait for 0.5 seconds

        //testing for player2
        for(int i = 0; i < 10; i++)
        {
            yield return new WaitForSeconds(0.1f);
            //Debug.Log("stopP2Movement: " + stopP2Movement);
        }

        if(player == 1)
        {
            UpdateStopMovement(1, false, 1);
            UpdateStopMovementY(1, false, 1);
        }
        else if(player == 2)
        {
            UpdateStopMovement(2, false, 1);
            UpdateStopMovementY(2, false, 1);
        }

        //Debug.Log("Knockback timer finished for player " + player);
    }

    //script int is used to identify which script is calling the function
    // 0:PlayerAttack 1:GameManager
    public void UpdateStopMovement(int player, bool stopMovement, int script)
    {
        player--; // Convert to 0-based index

        knockbackFrozen[player][script] = stopMovement;

        //if freezing, freeze
        if(stopMovement)
        {
            if(player == 0)
            {
                stopP1Movement = true;
            }
            else if(player == 1)
            {
                stopP2Movement = true;
            }
        }
        else if(player == 0) //potentially unfreezing, if all are false
        {
            if(knockbackFrozen[0][0] == false && knockbackFrozen[0][1] == false)
            {
                stopP1Movement = false;
            }
        }
        else if(player == 1)
        {
            if(knockbackFrozen[1][0] == false && knockbackFrozen[1][1] == false)
            {
                stopP2Movement = false;
            }
        }
    }

    //same but for y movement
    public void UpdateStopMovementY(int player, bool stopMovement, int script)
    {
        player--; // Convert to 0-based index

        knockbackFrozenY[player][script] = stopMovement;

        //if freezing, freeze
        if(stopMovement)
        {
            if(player == 0)
            {
                stopP1YMovement = true;
            }
            else if(player == 1)
            {
                stopP2YMovement = true;
            }
        }
        else if(player == 0) //potentially unfreezing, if all are false
        {
            if(knockbackFrozenY[0][0] == false && knockbackFrozenY[0][1] == false)
            {
                stopP1YMovement = false;
            }
        }
        else if(player == 1)
        {
            if(knockbackFrozenY[1][0] == false && knockbackFrozenY[1][1] == false)
            {
                stopP2YMovement = false;
            }
        }
    }

    public void playerJoined()
    {
        curPlayerCount++;
    }
    public void playerLeft()
    {
        curPlayerCount--;
    }
}
