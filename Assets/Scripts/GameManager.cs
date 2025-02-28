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
    public GameObject player1;
    public float P1AttackKnockBackX;
    public float P1AttackKnockBackY;
    public Boolean stopP1Movement;
    public Boolean stopP1YMovement;
    public Boolean canInputP1;
    public Boolean isHitBoxAnimatingP1;
    public Slider playerHealthBar;
    public int P1Health = 100;
    public Slider p1OffensiveBar;
    public static int p1WinCounter = 0;
    public bool player1IsOnGround;
    public bool p1Aggro;

    [Header("Player2")]
    public GameObject player2;
    public float P2AttackKnockBackX;
    public float P2AttackKnockBackY;
    public Boolean stopP2Movement;
    public Boolean stopP2YMovement;
    public Boolean canInputP2;
    public Boolean isHitBoxAnimatingP2;
    public bool P2Aggro;
    public Slider opponentHealth;
    public int P2Health = 100;
    public Slider p2Offensive;
    public static int p2WinCounter = 0;
    public bool player2IsOnGround;

    [Header("Offensive mode")]
    public float maxOffensiveBarValue;
    private float offensiveTimer1 = 0f;
    private float offensiveTimer2 = 0f;
    private float offensiveInterval = 1f;
    public int offensiveIncrease = 50;
    public int offensiveDecrease = 50;
    public int offensiveValueP1;
    public int offensiveValueP2;
    public bool isOffensiveP2 = false;
    public bool isOffensiveP1 = false;

    [Header("Text")]
    public TMP_Text winMessage;
    public TMP_Text countdownText;
    public TMP_Text timerText;
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
    public Button restartButton;
    public Button quitButton;

    [Header("Bools")]
    public bool isCountingDown;
    public bool roundOver = false;
    public bool gameOver = false;
    public bool p1Win = false;
    public bool p2Win = false;

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
        player1IsOnGround = false;
        player2IsOnGround = false;
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

        StartCoroutine(StartRoundCountdown(3));
    }

    void Update()
    {
        if (isCountingDown) return;
        offensiveTimer1 += Time.deltaTime;
        offensiveTimer2 += Time.deltaTime;

        if (offensiveTimer1 >= offensiveInterval)
        {
            if (!isOffensiveP1)
            {
                IncreaseOffensiveSlider(p1OffensiveBar);
            }
            else
            {
                DecreaseOffensiveSlider(p1OffensiveBar, ref isOffensiveP1);
            }
            offensiveTimer1 = 0;
        }

        if (offensiveTimer2 >= offensiveInterval)
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

    private void DecreaseOffensiveSlider(Slider offensiveSlider, ref bool isoffensive)
    {
        if (!roundOver && !gameOver)
        {
            if (isCountingDown) return;
            offensiveSlider.value = Mathf.Max(offensiveSlider.value - offensiveDecrease, 0);

            if (offensiveSlider.value == 0)
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
            offensiveSlider.value = Mathf.Min(offensiveSlider.value + offensiveIncrease, offensiveSlider.maxValue);
        }
    }

    public void Player1HitsPlayer2()
    {
        if (isCountingDown) return;
        P2Health -= 10;
        opponentHealth.value = P2Health;
        if (!isOffensiveP1)
        {
            p1OffensiveBar.value = Mathf.Min(p1OffensiveBar.value + offensiveIncrease, p1OffensiveBar.maxValue);
        }

        if (isOffensiveP1 && offensiveValueP1 > 0)
        {
            p1OffensiveBar.value = Mathf.Min(p1OffensiveBar.value + offensiveValueP1, p1OffensiveBar.maxValue);
            offensiveValueP1 -= 15;
        }

        if (isOffensiveP2)
        {
            p2Offensive.value = Mathf.Max(p2Offensive.value - offensiveIncrease, 0);
        }
    }

    public void Player2HitsPlayer1()
    {
        if (isCountingDown) return;
        P1Health -= 10;
        playerHealthBar.value = P1Health;
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
            roundOver = true;
            p1Win = true;
            p1WinCounter += 1;
            winMessage.text = "Player 1 Wins!";
            winMessage.gameObject.SetActive(true);
            winBox1P1.gameObject.SetActive(true);

            if (p1WinCounter >= 2)
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
            roundOver = true;
            p2Win = true;
            p2WinCounter += 1;
            winMessage.text = "Player 2 Wins!";
            winMessage.gameObject.SetActive(true);
            winBox1P2.gameObject.SetActive(true);

            if (p2WinCounter >= 2)
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

    private IEnumerator RestartMatch(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        yield return new WaitForSeconds(waitTime);

        winMessage.gameObject.SetActive(false);
        timerSeconds = 99f;
        countdownText.text = "99";
        P1Health = 100;
        P2Health = 100;
        playerHealthBar.value = P1Health;
        opponentHealth.value = P2Health;
        p1OffensiveBar.value = 0;
        p2Offensive.value = 0;
        isOffensiveP1 = false;
        isOffensiveP2 = false;
        p1Win = false;
        p2Win = false;
        roundOver = false;
        gameOver = false;
        player1.transform.position = new Vector2(-4, -3);
        player2.transform.position = new Vector2(4, -3);
        StartCoroutine(StartRoundCountdown(3));
    }

    public void RestartGame()
    {
        p1WinCounter = 0;
        p2WinCounter = 0;
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
        StartCoroutine(RestartMatch(2f));
    }

    private IEnumerator StartRoundCountdown(int countdownTime)
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
                yield break;
            }

            timerText.text = timerSeconds.ToString("F0");
            yield return new WaitForSeconds(1f);
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
