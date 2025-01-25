using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    private PlayerInputManager inputManager;
    private List<GameObject> players = new List<GameObject>();

    void Start()
    {
        //subscribes the method to the inputManager event
        inputManager = GetComponent<PlayerInputManager>();
        inputManager.onPlayerJoined += OnPlayerJoined;

        players.Add(GameManager.Instance.player1);
        players.Add(GameManager.Instance.player2);
    }

    private void OnPlayerJoined(PlayerInput playerInput)
    {
        Debug.Log("Player joined: " + playerInput.playerIndex);
        //players[playerInput.playerIndex].GetComponent<PlayerInput>().devices = new InputDevice[] { Gamepad.all[playerInput.playerIndex] };
    }
}
